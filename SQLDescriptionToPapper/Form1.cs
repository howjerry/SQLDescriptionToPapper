using Dapper;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SQLDescriptionToPapper
{
    public enum SearchType { Single, All, Like }

    public enum FormatType { Excel, Word }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConnectionStringSettingsCollection connectionconfig = ConfigurationManager.ConnectionStrings;
            IDictionary<string, string> connectionDicitionary = new Dictionary<string, string>();
            foreach (ConnectionStringSettings item in connectionconfig)
            {
                connectionDicitionary.Add(item.Name, item.ConnectionString);
            }

            cb_ConnectionString.DataSource = connectionDicitionary.Select(o => new { key = o.Key, value = o.Value }).ToList();
            cb_ConnectionString.DisplayMember = "key";
            cb_ConnectionString.ValueMember = "value";

            IDictionary<string, string> formatDicitionary = new Dictionary<string, string>();
            formatDicitionary.Add("Excel", "Excel");
            formatDicitionary.Add("Word", "Word");
            cb_Format.DataSource = formatDicitionary.Select(o => new { key = o.Key, value = o.Value }).ToList();
            cb_Format.DisplayMember = "key";
            cb_Format.ValueMember = "value";
        }

        private void bt_Select_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            this.txt_File.Text = path.SelectedPath;
        }

        private void bt_Send_Click(object sender, EventArgs e)
        {
            IDbConnection cn = new SqlConnection(cb_ConnectionString.SelectedValue.ToString());

            try
            {
                var choiceSearchType = grp_Search.Controls.OfType<RadioButton>().Single(x => x.Checked);
                var searchType = (SearchType)Enum.Parse(typeof(SearchType), choiceSearchType.Tag.ToString());
                StringBuilder tableWhr = new StringBuilder();

                tableWhr.Append("where obj.xtype = 'U'");

                switch (searchType)
                {
                    case SearchType.Single:
                        if (string.IsNullOrWhiteSpace(txt_TableName.Text))
                            throw new ArgumentNullException("請輸入資料表名稱");
                        tableWhr.AppendFormat("and obj.name = '{0}'", txt_TableName.Text);
                        break;
                    case SearchType.All:
                        break;
                    case SearchType.Like:
                        if (string.IsNullOrWhiteSpace(txt_TableName.Text))
                            throw new ArgumentNullException("請輸入資料表名稱");
                        tableWhr.AppendFormat("and obj.name LIKE '%{0}%'", txt_TableName.Text);
                        break;
                    default:
                        break;
                }

                string SQL_Fst =
                   @"select
                	obj.name [table_name],
                	dsc.value [Description],
                	index_col(obj.name, pk.indid, 1) [col_1],
                	index_col(obj.name, pk.indid, 2) [col_2],
                	index_col(obj.name, pk.indid, 3) [col_3],
                	index_col(obj.name, pk.indid, 4) [col_4],
                	index_col(obj.name, pk.indid, 5) [col_5],
                	index_col(obj.name, pk.indid, 6) [col_6],
                	index_col(obj.name, pk.indid, 7) [col_7],
                	index_col(obj.name, pk.indid, 8) [col_8]
                from sysobjects obj
                left join (
                	select *
                	from ::fn_listextendedproperty(default, 'USER', 'dbo', 'TABLE', default, default, default)
                	where name = 'MS_Description'
                ) dsc on dsc.objname COLLATE database_default = obj.name
                left join (
                	select k.parent_obj, k.[name], ix.indid
                	from sysobjects k
                	left join sysindexes ix on ix.[name] = k.[name]
                	where xtype = 'PK'
                ) pk on pk.parent_obj = object_id(obj.name)
                " + tableWhr.ToString();

                string SQL_Sec =
                   @"select ordinal_position,
               	'{0}' [table_name], column_name,
               	Case when data_type='numeric' then data_type +'('+convert(varchar,numeric_precision)+','+convert(varchar,numeric_scale)+')' when data_type like '%text%' then data_type when character_maximum_length is not null then data_type +'('+convert(varchar,character_maximum_length)+')' else data_type End [DataType],
               	Case is_nullable when 'YES' then 'Null' else 'Not Null' End [NullAble],
               	ext.value [Description]
               from information_schema.columns ba
               left join (
               	select *
               	from ::fn_listextendedproperty(null, 'USER', 'dbo', 'TABLE', '{0}', 'COLUMN', default)
               ) ext on ext.objname COLLATE database_default = ba.column_name
               where table_name = '{0}'
               ";

                DataTable tabTable = new DataTable();
                tabTable.TableName = "Tab_Table";
                tabTable.Load(cn.ExecuteReader(SQL_Fst));
                cn.Close();

                if (tabTable.Rows.Count == 0)
                {
                    lb_Message.Text = "查無相關資料表，請確認您的輸入的資料表名稱！";
                    return;
                }

                List<string> sec_list = new List<string>();

                for (int i = 0; i < tabTable.Rows.Count; i++)
                {
                    sec_list.Add(string.Format(SQL_Sec, tabTable.Rows[i]["table_name"].ToString()));
                }

                string sql = string.Join("union all" + "\r\n", sec_list.ToArray()) + "\r\n";


                DataTable colTable = new DataTable();
                colTable.Load(cn.ExecuteReader(sql));
                colTable.TableName = "Col_Table";

                DataSet objDs = new DataSet();
                objDs.Tables.Add(tabTable);
                objDs.Tables.Add(colTable);

                DataRelation objRe = new DataRelation("ReCol", objDs.Tables["Tab_Table"].Columns["table_name"], objDs.Tables["Col_Table"].Columns["table_name"]);
                objDs.Relations.Add(objRe);
                var formatType = (FormatType)Enum.Parse(typeof(FormatType), cb_Format.SelectedValue.ToString());

                switch (formatType)
                {
                    case FormatType.Excel:
                        RenderExcel(tabTable, colTable);
                        break;
                    case FormatType.Word:
                        RenderWord(tabTable, colTable);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                lb_Message.Text = ex.ToString();
            }
            finally
            {
                cn.Close();
                cn.Dispose();
            }

            MessageBox.Show("完成");
        }

        private void RenderWord(DataTable tabTable, DataTable colTable)
        {
            StringBuilder DocBuilder = new StringBuilder();

            for (int i = 0; i < tabTable.Rows.Count; i++)
            {
                DocBuilder.AppendFormat(
@"<h1 align=center style='text-align:center'>
    <![if !supportLists]>
        <span lang=EN-US style='font-size:18.0pt;font-family:標楷體;mso-bidi-font-family:標楷體'>
	        <span style='mso-list:Ignore'>表一　</span>
        </span>
    <![endif]>
    <span style='font-size:18.0pt; font-family:標楷體;mso-bidi-font-family:""Times New Roman""'>
	    {0}
	    <span lang=EN-US>	
		    <o:p></o:p>
	    </span>
    </span>
</h1>", tabTable.Rows[i]["Description"].ToString());

                DocBuilder.Append("<table class=\"MsoNormalTable\" border=\"1\" style=\"width:100%; border-collapse:collapse;\">");
                DocBuilder.Append("<tr>");
                DocBuilder.Append("<td colspan=\"4\">");
                DocBuilder.AppendFormat("資料表名稱：{0}", tabTable.Rows[i]["table_name"].ToString());
                DocBuilder.Append("</td>");
                DocBuilder.Append("</tr>");

                DocBuilder.Append("<tr>");
                DocBuilder.Append("<td colspan=\"4\">");
                DocBuilder.AppendFormat("資料表說明：{0}", tabTable.Rows[i]["Description"].ToString());
                DocBuilder.Append("</td>");
                DocBuilder.Append("</tr>");

                List<string> pkList = new List<string>();
                for (int pk = 1; pk <= 8; pk++)
                {
                    if (tabTable.Rows[i]["col_" + pk.ToString()] != DBNull.Value)
                    {
                        pkList.Add(tabTable.Rows[i]["col_" + pk.ToString()].ToString());
                    }
                }

                string PkStr = "無";
                if (pkList.Count > 0)
                {
                    PkStr = string.Join("、", pkList.ToArray());
                }

                DocBuilder.Append("<tr>");
                DocBuilder.Append("<td colspan=\"4\">");
                DocBuilder.AppendFormat("主鍵欄位：{0}", PkStr);
                DocBuilder.Append("</td>");
                DocBuilder.Append("</tr>");

                DocBuilder.Append("<tr>");
                DocBuilder.Append("<td align=\"center\">欄位名稱<br>(Field Name)</td>");
                DocBuilder.Append("<td align=\"center\">格式<br>(Format)</td>");
                DocBuilder.Append("<td align=\"center\">有效值<br>(Validation)</td>");
                DocBuilder.Append("<td align=\"center\">欄位說明<br>(Description)</td>");
                DocBuilder.Append("</tr>");

                DataRow[] drs = colTable.Select("table_name = '" + tabTable.Rows[i]["table_name"].ToString() + "'", "ordinal_position Asc");

                for (int j = 0; j < drs.Length; j++)
                {
                    DocBuilder.Append("<tr>");

                    DocBuilder.Append("<td>");
                    DocBuilder.Append(drs[j]["column_name"].ToString() + "<br>");
                    DocBuilder.Append("</td>");

                    DocBuilder.Append("<td>");
                    DocBuilder.Append(drs[j]["DataType"].ToString() + "<br>");
                    DocBuilder.Append("</td>");

                    DocBuilder.Append("<td>");
                    DocBuilder.Append(drs[j]["NullAble"].ToString() + "<br>");
                    DocBuilder.Append("</td>");

                    DocBuilder.Append("<td>");
                    DocBuilder.Append(drs[j]["Description"].ToString() + "<br>");
                    DocBuilder.Append("</td>");

                    DocBuilder.Append("</tr>");
                }

                DocBuilder.Append("</table>");
                DocBuilder.Append("<br clear=\"all\" style=\"page-break-before:always\" />");
            }

            using (FileStream file = new FileStream(GetFileFullPath(".doc"), FileMode.Create))
            {
                StreamWriter writer = new StreamWriter(file);
                writer.Write(getWordHtml(DocBuilder.ToString()));
                file.Close();
            }
        }

        private void RenderExcel(DataTable tabTable, DataTable colTable)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();

            HSSFFont myFont = (HSSFFont)workbook.CreateFont();
            myFont.FontName = "Times New Roman";
            myFont.FontHeightInPoints = 12;

            HSSFCellStyle itemStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            itemStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            itemStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            itemStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            itemStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            itemStyle.SetFont(myFont);

            HSSFCellStyle headerStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            headerStyle.CloneStyleFrom(itemStyle);
            headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

            for (int i = 0; i < tabTable.Rows.Count; i++)
            {
                ISheet newSheet = workbook.CreateSheet(tabTable.Rows[i]["table_name"].ToString());
                IRow xlsRow = null;
                ICell xlsCell = null;

                xlsRow = newSheet.CreateRow(0);

                xlsCell = xlsRow.CreateCell(0);
                xlsCell.SetCellValue("欄位名稱");
                xlsCell.CellStyle = headerStyle;

                xlsCell = xlsRow.CreateCell(1);
                xlsCell.SetCellValue("格式");
                xlsCell.CellStyle = headerStyle;

                xlsCell = xlsRow.CreateCell(2);
                xlsCell.SetCellValue("有效值");
                xlsCell.CellStyle = headerStyle;

                xlsCell = xlsRow.CreateCell(3);
                xlsCell.SetCellValue("欄位說明");
                xlsCell.CellStyle = headerStyle;

                DataRow[] drs = colTable.Select("table_name = '" + tabTable.Rows[i]["table_name"].ToString() + "'", "ordinal_position Asc");

                for (int j = 0; j < drs.Length; j++)
                {
                    xlsRow = newSheet.CreateRow(j + 1);

                    xlsCell = xlsRow.CreateCell(0);
                    xlsCell.SetCellValue(drs[j]["column_name"].ToString());
                    xlsCell.CellStyle = itemStyle;

                    xlsCell = xlsRow.CreateCell(1);
                    xlsCell.SetCellValue(drs[j]["DataType"].ToString());
                    xlsCell.CellStyle = itemStyle;

                    xlsCell = xlsRow.CreateCell(2);
                    xlsCell.SetCellValue(drs[j]["NullAble"].ToString());
                    xlsCell.CellStyle = itemStyle;

                    xlsCell = xlsRow.CreateCell(3);
                    xlsCell.SetCellValue(drs[j]["Description"].ToString());
                    xlsCell.CellStyle = itemStyle;
                }

                newSheet.AutoSizeColumn(0);
                newSheet.AutoSizeColumn(1);
                newSheet.AutoSizeColumn(2);
                newSheet.AutoSizeColumn(3);
            }

            using (FileStream file = new FileStream(GetFileFullPath(".xls"), FileMode.Create))
            {
                workbook.Write(file);
                file.Close();
            }
        }

        private string getWordHtml(string content)
        {
            string result =
    @"<html xmlns:o=""urn:schemas-microsoft-com:office:office""
xmlns:w=""urn:schemas-microsoft-com:office:word""
xmlns=""http://www.w3.org/TR/REC-html40"">
<head>
<meta http-equiv=Content-Type content=""text/html; charset=utf-8"">
<meta name=ProgId content=Word.Document>
<!--[if gte mso 9]>
<xml>
	<w:WordDocument>
		<w:View>Print</w:View>
		<w:PunctuationKerning/>
		<w:DisplayHorizontalDrawingGridEvery>0</w:DisplayHorizontalDrawingGridEvery>
		<w:DisplayVerticalDrawingGridEvery>2</w:DisplayVerticalDrawingGridEvery>
		<w:Compatibility>
			<w:SpaceForUL/>
			<w:BalanceSingleByteDoubleByteWidth/>
			<w:DoNotLeaveBackslashAlone/>
			<w:ULTrailSpace/>
			<w:DoNotExpandShiftReturn/>
			<w:AdjustLineHeightInTable/>
			<w:BreakWrappedTables/>
			<w:SnapToGridInCell/>
			<w:WrapTextWithPunct/>
			<w:UseAsianBreakRules/>
			<w:UseFELayout/>
		</w:Compatibility>
		<w:BrowserLevel>MicrosoftInternetExplorer4</w:BrowserLevel>
	</w:WordDocument>
</xml>
<![endif]-->
<!--[if gte mso 9]>
<xml>
    <w:LatentStyles DefLockedState=""false"" LatentStyleCount=""156"">
    </w:LatentStyles>
</xml>
<![endif]-->
<style>
<!--
h1
	{mso-margin-top-alt:auto;
	margin-right:0cm;
	mso-margin-bottom-alt:auto;
	margin-left:21.25pt;
	text-indent:-21.25pt;
	mso-pagination:widow-orphan;
	mso-outline-level:1;
	mso-list:l0 level1 lfo1;
	font-size:14.0pt;
	font-family:標楷體;
	mso-bidi-font-family:標楷體;}
/* Page Definitions */
@page
	{mso-page-border-surround-header:no;
	mso-page-border-surround-footer:no;}
@page Section1
	{size:21.0cm 842.0pt;
	mso-page-orientation:landscape;
	margin:2.0cm 2.0cm 2.0cm 2.0cm;
	mso-header-margin:42.55pt;
	mso-footer-margin:42.55pt;
	mso-paper-source:0;}
div.Section1
	{page:Section1;}
/* List Definitions */
@list l0
	{mso-list-id:57561321;
	mso-list-template-ids:-508032836;}
@list l0:level1
	{mso-level-number-format:taiwanese-counting-thousand;
	mso-level-style-link:""標題 1"";
	mso-level-suffix:none;
	mso-level-text:表%1　;
	mso-level-tab-stop:none;
	mso-level-number-position:left;
	margin-left:21.25pt;
	text-indent:-21.25pt;}
ol
	{margin-bottom:0cm;}
ul
	{margin-bottom:0cm;}
-->
</style>
<!--[if gte mso 10]>
<style>
 /* Style Definitions */
 table.MsoNormalTable
	{mso-style-name:表格內文;
	mso-tstyle-rowband-size:0;
	mso-tstyle-colband-size:0;
	mso-style-noshow:yes;
	mso-level-text:"""";
	mso-padding-alt:0cm 5.4pt 0cm 5.4pt;
	mso-para-margin:0cm;
	mso-para-margin-bottom:.0001pt;
	mso-pagination:widow-orphan;
	font-size:10.0pt;
	font-family:""Times New Roman"";
	mso-fareast-font-family:""標楷體"";
	mso-ansi-language:#0400;
	mso-fareast-language:#0400;
	mso-bidi-language:#0400;}
</style>
<![endif]-->
</head>
<body>
    <div class=""albert"">" +
            content + @"
    </div>
</body>
</html>";

            return result;
        }

        private string GetFileFullPath(string extension)
        {
            var path = txt_File.Text;

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("未選擇路徑。");

            var fileNameOnly = string.IsNullOrWhiteSpace(txt_TableName.Text)
            ? cb_ConnectionString.Text
            : txt_TableName.Text;

            var fullPath = Path.Combine(path, fileNameOnly + extension);
            int count = 1;

            while (File.Exists(fullPath))
            {
                string tempFileName = string.Format("{0}({1}){2}", fileNameOnly, count++, extension);
                fullPath = Path.Combine(path, tempFileName);
            }

            return fullPath;
        }
    }
}

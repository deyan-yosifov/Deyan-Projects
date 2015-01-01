/**
 * Retrieves all the rows in the active spreadsheet that contain data and logs the
 * values for each row.
 * For more information on using the Spreadsheet API, see
 * https://developers.google.com/apps-script/service_spreadsheet
 */
function testCode() {
  var sheet = SpreadsheetApp.getActiveSheet();
  var rows = sheet.getDataRange();
  var numRows = rows.getNumRows();
  var values = rows.getValues();

  var text = "";
  
  for (var i = 0; i <= numRows - 1; i++) {
    var row = values[i];
    text += row + " row data\n";
    //Logger.log(row);
  }
  
  var ui = SpreadsheetApp.getUi();
  
  var result = ui.alert(
     'See rows data!',
      text,
      ui.ButtonSet.YES_NO);
};

function logObjectProperties(instance)
{  
  for(var property in instance) {
    Logger.log(property);
  }
};

function logAsJSON(instance)
{  
  Logger.log(JSON.stringify(instance));
};

var reportSheet = null;

var reportConstants = {
  regex: {
    deyan: '"*Деян*"',    
    radostina: '"*Радостина*"',
    personalExpense: '"*Личен разход*"',
    mutualExpense: '"*Общ разход*"',
  },
  rows: {    
    header: "1",
    start: "2",
    end: "1000",
  },
  columns: {
    value: "A",
    description: "B",
    ownerName: "C",
    type: "D",
    deyanHelper: "E",
    radostinaHelper: "F",
    resultHeaders: "H",
    resultTotal: "I",
    resultDeyan: "J",
    resultRadostina: "K"
  },
};

function guardVariables()
{
  if(!reportSheet)
  {
    throw { "message" : "You should set active sheet as report sheet first!" };
  }
};

function setCellValue(range, value)
{
  guardVariables();
  reportSheet.getRange(range).setValue(value);
};

function getRangeText(rStart, cStart, rEnd, cEnd)
{
  rStart = reportConstants.rows[rStart] || rStart;
  cStart = reportConstants.columns[cStart] || cStart;
  
  var range = "";
  
  if(arguments.length > 2)
  {
    rEnd = reportConstants.rows[rEnd] || rEnd;
    cEnd = reportConstants.columns[cEnd] || cEnd;
    range += cStart + rStart + ":" + cEnd + rEnd;
  }
  else
  {
    range += cStart + rStart;
  }  
  
  return range;
};

function generateSheet() {  
  reportSheet = SpreadsheetApp.getActiveSheet();
  var cellValue = null;
  
  setCellValue(getRangeText("header", "value"), "Стойност в лв.");
  setCellValue(getRangeText("header", "description"), "Описание на приход/разход");
  setCellValue(getRangeText("header", "ownerName"), "Кой?");
  setCellValue(getRangeText("header", "type"), "Вид приход/разход");
  setCellValue(getRangeText("header", "deyanHelper"), "Деян");
  setCellValue(getRangeText("header", "radostinaHelper"), "Радостина");
  
  cellValue = '=SUMIF(' + getRangeText('start','ownerName') + ', ' + reportConstants.regex.deyan + ', ' + getRangeText('start','value') + ')';
  setCellValue(getRangeText("start", "deyanHelper", "end", "deyanHelper"), cellValue);
  
  cellValue = '=SUMIF(' + getRangeText('start','ownerName') + ', ' + reportConstants.regex.radostina + ', ' + getRangeText('start','value') + ')';
  setCellValue(getRangeText("start", "radostinaHelper", "end", "radostinaHelper"), cellValue);
  
};

/**
 * Adds a custom menu to the active spreadsheet, containing a single menu item
 * for invoking the testCode() function specified above.
 * The onOpen() function, when defined, is automatically invoked whenever the
 * spreadsheet is opened.
 * For more information on using the Spreadsheet API, see
 * https://developers.google.com/apps-script/service_spreadsheet
 */
function onOpen() {
  var spreadsheet = SpreadsheetApp.getActiveSpreadsheet();
  var entries = [
    {
      name : "Test code",
      functionName : "testCode"
    },
    {
      name : "Generate sheet",
      functionName : "generateSheet"
    },
  ];
  spreadsheet.addMenu("Incomes/Expenses", entries);
};

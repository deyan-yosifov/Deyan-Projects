var oneHourMilliseconds = 60 * 60 * 1000;
var oneDayMilliseconds = 24 * oneHourMilliseconds;
var monthNames = ['Януари', 'Февруари', 'Март', 'Април', 'Май', 'Юни', 'Юли', 'Август', 'Септември', 'Октомври', 'Ноември', 'Декември'];

function getNextDate(date) {
  var nextDate = new Date(date.getTime() + oneDayMilliseconds);
  
  if (nextDate.getHours() != date.getHours()) {
    // This is done because of daylight savings!
    nextDate = new Date(nextDate.getTime() + (nextDate.getHours() - date.getHours()) * oneHourMilliseconds);
  }
  
  return nextDate;
};

function getWeeksInMonth(date){
  var month = date.getMonth();
  var weeks = [];
  var startDate = new Date(date.getFullYear(), month, 1);
  var currentDate = startDate;
  var nextDate = getNextDate(currentDate);
  
  while(nextDate.getMonth() == month)
  {
    if(nextDate.getDay() == 1){
      weeks.push({
        start: startDate,
        end: currentDate,
      });
      
      startDate = nextDate;      
    }
    
    currentDate = nextDate;
    nextDate = getNextDate(currentDate);
  }
  
  weeks.push({
    start: startDate,
    end: currentDate,
  });
    
  return weeks;
};

function getWeekIndex(date){
  var weeks = getWeeksInMonth(date);
  var dateNum = date.getDate();
  
  for(var i = 0; i < weeks.length; i+=1){
    var week = weeks[i];    
    if(week.start.getDate() <= dateNum && dateNum <= week.end.getDate()){
     return  i;
    }
  }
  
  throw {"message": "Calculations error! Cannot find week index!"};
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

function promptDate()
{
  var ui = SpreadsheetApp.getUi();
  
  var result = ui.prompt(
     'Choose some date!',
    "Sample date format: 31-1-2015",
      ui.ButtonSet.OK_CANCEL);  
  
  if(result.getSelectedButton() == ui.Button.OK){
    var text = result.getResponseText();
    var nums = text.split("-");
    var day = parseInt(nums[0]);
    var month = parseInt(nums[1]);
    var year = parseInt(nums[2]);
    var date = new Date(year, month - 1, day);
    return date;
  }
  else{
    return false;
  }
};

function alert(text)
{  
  var ui = SpreadsheetApp.getUi();
  ui.alert(text);
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
  var date = promptDate();
  
  if(!date)
  {
    alert("Date not selected!");
    return;
  }
  
  setCellValue(getRangeText("header", "resultTotal"), date);
  setCellValue(getRangeText("header", "resultDeyan"), getNextDate(date));
  
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
      name : "Generate sheet",
      functionName : "generateSheet"
    },
  ];
  spreadsheet.addMenu("Incomes/Expenses", entries);
};

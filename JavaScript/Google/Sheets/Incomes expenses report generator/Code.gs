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
    return parseDate(text);
  }
  else{
    return false;
  }
};

function parseDate(text){
  var nums = text.split("-");
  var day = parseInt(nums[0]);
  var month = parseInt(nums[1]);
  var year = parseInt(nums[2]);
  var date = new Date(year, month - 1, day);
  
  return date;
};

function alert(text)
{  
  var ui = SpreadsheetApp.getUi();
  ui.alert(text);
};

var reportSheet = null;

var reportConstants = {  
  tablesRowOffset: 12,
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

function getRangeText(rStart, cStart, rEnd, cEnd, isAbsolute)
{
  rStart = reportConstants.rows[rStart] || rStart;
  cStart = reportConstants.columns[cStart] || cStart;
  dolar = (isAbsolute ? "$" : "");
  
  var range = "";
  
  if(rEnd && cEnd)
  {
    rEnd = reportConstants.rows[rEnd] || rEnd;
    cEnd = reportConstants.columns[cEnd] || cEnd;
    range += dolar + cStart + dolar + rStart + ":" + dolar + cEnd + dolar + rEnd;
  }
  else
  {
    range += dolar + cStart + dolar + rStart;
  }  
  
  return range;
};

function generateStatisticTable(json){
  var valuesStart = json.valuesStart;
  var valuesEnd = json.valuesEnd;
  var tableStartRow = parseInt(json.tableStartRow);
  var tableHeader = json.tableHeader;
  var shouldAddCostsEqualizations = json.shouldAddCostsEqualizations;
  
  var valueRange = getRangeText(valuesStart, "value", valuesEnd, "value");
  var typeRange = getRangeText(valuesStart, "type", valuesEnd, "type");
  var deyanRange = getRangeText(valuesStart, "deyanHelper", valuesEnd, "deyanHelper");
  var radiRange = getRangeText(valuesStart, "radostinaHelper", valuesEnd, "radostinaHelper");
  var personalExpense = reportConstants.regex.personalExpense;
  var mutualExpense = reportConstants.regex.mutualExpense;
  
  if(tableHeader){
   setCellValue(getRangeText(tableStartRow-1, "resultHeaders"), tableHeader);
  }
  
  var cellValue = null;
  
  var currentRow = tableStartRow;
  setCellValue(getRangeText(currentRow, "resultHeaders"), "Баланс");
  cellValue = "=SUM(" + getRangeText(currentRow + 1, "resultTotal", currentRow + 2, "resultTotal") + ")";
  setCellValue(getRangeText(currentRow, "resultTotal"), cellValue);
  cellValue = "=SUM(" + getRangeText(currentRow + 1, "resultDeyan", currentRow + 2, "resultDeyan") + ")";
  setCellValue(getRangeText(currentRow, "resultDeyan"), cellValue);
  cellValue = "=SUM(" + getRangeText(currentRow + 1, "resultRadostina", currentRow + 2, "resultRadostina") + ")";
  setCellValue(getRangeText(currentRow, "resultRadostina"), cellValue);
  
  currentRow++;
  setCellValue(getRangeText(currentRow, "resultHeaders"), "Приходи");
  cellValue = "=SUMIF(" + valueRange + ',">0")';
  setCellValue(getRangeText(currentRow, "resultTotal"), cellValue);
  cellValue = "=SUMIF(" + deyanRange + ',">0")';
  setCellValue(getRangeText(currentRow, "resultDeyan"), cellValue);
  cellValue = "=SUMIF(" + radiRange + ',">0")';
  setCellValue(getRangeText(currentRow, "resultRadostina"), cellValue);
    
  currentRow++;
  setCellValue(getRangeText(currentRow, "resultHeaders"), "Разходи");
  cellValue = "=SUMIF(" + valueRange + ',"<0")';
  setCellValue(getRangeText(currentRow, "resultTotal"), cellValue);
  cellValue = "=SUMIF(" + deyanRange + ',"<0")';
  setCellValue(getRangeText(currentRow, "resultDeyan"), cellValue);
  cellValue = "=SUMIF(" + radiRange + ',"<0")';
  setCellValue(getRangeText(currentRow, "resultRadostina"), cellValue);
  
  currentRow+=2;
  var mutualExpensesRow = currentRow;
  setCellValue(getRangeText(currentRow, "resultHeaders"), "Общ разход");
  cellValue = "=SUMIF(" + typeRange + ", " + mutualExpense + ", " + valueRange + ')';
  setCellValue(getRangeText(currentRow, "resultTotal"), cellValue);
  cellValue = "=SUMIF(" + typeRange + ", " + mutualExpense + ", " + deyanRange + ')';
  setCellValue(getRangeText(currentRow, "resultDeyan"), cellValue);
  cellValue = "=SUMIF(" + typeRange + ", " + mutualExpense + ", " + radiRange + ')';
  setCellValue(getRangeText(currentRow, "resultRadostina"), cellValue);
  
  currentRow++;
  setCellValue(getRangeText(currentRow, "resultHeaders"), "Личен разход");
  cellValue = "=SUMIF(" + typeRange + ", " + personalExpense + ", " + valueRange + ')';
  setCellValue(getRangeText(currentRow, "resultTotal"), cellValue);
  cellValue = "=SUMIF(" + typeRange + ", " + personalExpense + ", " + deyanRange + ')';
  setCellValue(getRangeText(currentRow, "resultDeyan"), cellValue);
  cellValue = "=SUMIF(" + typeRange + ", " + personalExpense + ", " + radiRange + ')';
  setCellValue(getRangeText(currentRow, "resultRadostina"), cellValue);
  
  currentRow+=2;
  var incomesRow = 3;
  var totalIncomesRange = getRangeText(incomesRow, "resultTotal", null, null, true);
  setCellValue(getRangeText(currentRow, "resultHeaders"), "Процент приход");  
  cellValue = "=" + totalIncomesRange + "/" + totalIncomesRange;
  setCellValue(getRangeText(currentRow, "resultTotal"), cellValue);
  cellValue = "=" + getRangeText(incomesRow, "resultDeyan", null, null, true) + "/" + totalIncomesRange;
  setCellValue(getRangeText(currentRow, "resultDeyan"), cellValue);
  cellValue = "=" + getRangeText(incomesRow, "resultRadostina", null, null, true) + "/" + totalIncomesRange;
  setCellValue(getRangeText(currentRow, "resultRadostina"), cellValue);
  
  currentRow++;
  var totalMutualExpensesRange = getRangeText(mutualExpensesRow, "resultTotal");
  setCellValue(getRangeText(currentRow, "resultHeaders"), "Процент общ разход");  
  cellValue = "=" + totalMutualExpensesRange + "/" + totalMutualExpensesRange;
  setCellValue(getRangeText(currentRow, "resultTotal"), cellValue);
  cellValue = "=" + getRangeText(mutualExpensesRow, "resultDeyan") + "/" + totalMutualExpensesRange;
  setCellValue(getRangeText(currentRow, "resultDeyan"), cellValue);
  cellValue = "=" + getRangeText(mutualExpensesRow, "resultRadostina") + "/" + totalMutualExpensesRange;
  setCellValue(getRangeText(currentRow, "resultRadostina"), cellValue);
  
  if(shouldAddCostsEqualizations){
    currentRow++;
    var totalAbsEnding = ")*ABS(" + totalMutualExpensesRange + ")";
    setCellValue(getRangeText(currentRow, "resultHeaders"), "Изравняване на разходи");  
    cellValue = "=(" + getRangeText(currentRow-1, "resultDeyan") + "-" + getRangeText(currentRow-2, "resultDeyan") + totalAbsEnding;
    setCellValue(getRangeText(currentRow, "resultDeyan"), cellValue);
    cellValue = "=(" + getRangeText(currentRow-1, "resultRadostina") + "-" + getRangeText(currentRow-2, "resultRadostina") + totalAbsEnding;
    setCellValue(getRangeText(currentRow, "resultRadostina"), cellValue);
  }  
};

function generateSheet() {      
  reportSheet = SpreadsheetApp.getActiveSheet();
  var cellValue = null;  
  var date = promptDate();
//  var date = parseDate("2-1-2015");
  
  if(!date)
  {
    alert("Date not selected!");
    return;
  }
    
  setCellValue(getRangeText("header", "value"), "Стойност в лв.");
  setCellValue(getRangeText("header", "description"), "Описание на приход/разход");
  setCellValue(getRangeText("header", "ownerName"), "Кой?");
  setCellValue(getRangeText("header", "type"), "Вид приход/разход");
  setCellValue(getRangeText("header", "deyanHelper"), "Деян");
  setCellValue(getRangeText("header", "radostinaHelper"), "Радостина");
  setCellValue(getRangeText("header", "resultTotal"), "Общи резултати");
  setCellValue(getRangeText("header", "resultDeyan"), "Деян");
  setCellValue(getRangeText("header", "resultRadostina"), "Радостина");
  
  cellValue = '=SUMIF(' + getRangeText('start','ownerName') + ', ' + reportConstants.regex.deyan + ', ' + getRangeText('start','value') + ')';
  setCellValue(getRangeText("start", "deyanHelper", "end", "deyanHelper"), cellValue);
  
  cellValue = '=SUMIF(' + getRangeText('start','ownerName') + ', ' + reportConstants.regex.radostina + ', ' + getRangeText('start','value') + ')';
  setCellValue(getRangeText("start", "radostinaHelper", "end", "radostinaHelper"), cellValue);
  
  var valuesStart = reportConstants.rows.start;
  var valuesEnd = reportConstants.rows.end;
  var tableStart = parseInt(valuesStart);
  var tableHeader = false;
  
  generateStatisticTable({
    valuesStart: valuesStart,
    valuesEnd: valuesEnd,
    tableStartRow: tableStart,
    tableHeader: tableHeader,
    shouldAddCostsEqualizations: true,
  });
  
  valuesStart = (parseInt(valuesEnd) - 1) + "";
  var weeks = getWeeksInMonth(date);
  var monthAndYear = monthNames[date.getMonth()] + " " + date.getFullYear();
  
  for(var i = 0; i < weeks.length; i++){
    var week = weeks[i];
    tableStart += reportConstants.tablesRowOffset;
    tableHeader = week.start.getDate() + "-" + week.end.getDate() + " " + monthAndYear;
    
    generateStatisticTable({
      valuesStart: valuesStart,
      valuesEnd: valuesEnd,
      tableStartRow: tableStart,
      tableHeader: tableHeader,
      shouldAddCostsEqualizations: false,
    });
  }  
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

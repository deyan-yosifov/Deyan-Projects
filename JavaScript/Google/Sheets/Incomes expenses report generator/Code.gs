var oneHourMilliseconds = 60 * 60 * 1000;
var oneDayMilliseconds = 24 * oneHourMilliseconds;
var monthNames = ['Януари', 'Февруари', 'Март', 'Април', 'Май', 'Юни', 'Юли', 'Август', 'Септември', 'Октомври', 'Ноември', 'Декември'];
var shouldGenerateWeekReports = false;

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

function stringifyDate(date){
	var day = date.getDate();
	var month = date.getMonth();
	var year = date.getFullYear();
	
	return day + "-" + (month + 1) + "-" + year;
};

function parseDate(text){
  var nums = text.split("-");
  var day = parseInt(nums[0]);
  var month = parseInt(nums[1]);
  var year = parseInt(nums[2]);
  var date = new Date(year, month - 1, day);
  
  return date;
};

function parseWeekTableInfo(text){
  var vals = text.split(" ");
  var range = vals[1].split(":");
  
  return {
    date: parseDate(vals[0]),
    fromValues: parseInt(range[0]),
    toValues: parseInt(range[1]),                           
  };
};

function alert(text)
{  
  var ui = SpreadsheetApp.getUi();
  ui.alert(text);
};

var reportSheet = null;

var reportConstants = {  
  appName: "Месечни приходи/разходи",
  tablesRowOffset: 12,
  ownerNames: {
	deyan: "Деян",
	radostina: "Радостина",
  },
  types: {
	income: "Приход",
	mutualExpense: "Общ разход",
	personalExpense: "Личен разход",  
  },
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
    resultRadostina: "K",    
    autoResizables: [1, 2, 4, 8, 9]
  },
};

function guardVariables()
{
  if(!reportSheet)
  {
    throw { "message" : "You should set active sheet as report sheet first!" };
  }
};

function getDisplayValue(range){
	guardVariables();
	var cellRange = reportSheet.getRange(range);
	var value = cellRange.getDisplayValue();
	
	return value;
};

function getFormulaValue(range){
	guardVariables();
	var cellRange = reportSheet.getRange(range);
	var value = cellRange.getFormula();
	
	return value;
};

function setCellValue(range, value, json)
{
  guardVariables();
  var cellRange = reportSheet.getRange(range);
  cellRange.setValue(value);
  
  if(json){
    if(json.fontWeight){
      cellRange.setFontWeight(json.fontWeight);
    }
    
    if(json.fontSize){
      cellRange.setFontSize(json.fontSize);
    }
    
    if(json.numberFormat){
      cellRange.setNumberFormat(json.numberFormat);
    }
  }
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

function getMonthAndYear(date){
  return monthNames[date.getMonth()] + " " + date.getFullYear();
};

function tryGetCurrentReportMonthDate(){
	guardVariables();
	var name = reportSheet.getName();	
	var monthAndYear = name.split(" ");
	var date = null;
	
	var isValid = monthAndYear.length == 2;
	
	if(isValid){
		var day = 1;
		var monthIndex = monthNames.indexOf(monthAndYear[0]);
		var year = parseInt(monthAndYear[1]);
		
		isValid = monthIndex > -1 && year > 2000;
		
		if(isValid){
			date = new Date(year, monthIndex, day);
		}
	}
		
	return date;	
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
    
  var bold = { fontWeight: "bold" };
  var percentFormat = { numberFormat: "0.00%" };
  var decimalFormat = { numberFormat: "#,##0.00" };
  var boldIf = (shouldAddCostsEqualizations ? bold : null);
  
  if(tableHeader){
   setCellValue(getRangeText(tableStartRow-1, "resultHeaders"), tableHeader, bold);
  }
  
  var cellValue = null;
  
  var currentRow = tableStartRow;
  setCellValue(getRangeText(currentRow, "resultHeaders"), "Баланс", boldIf);
  cellValue = "=SUM(" + getRangeText(currentRow + 1, "resultTotal", currentRow + 2, "resultTotal") + ")";
  setCellValue(getRangeText(currentRow, "resultTotal"), cellValue, decimalFormat);
  cellValue = "=SUM(" + getRangeText(currentRow + 1, "resultDeyan", currentRow + 2, "resultDeyan") + ")";
  setCellValue(getRangeText(currentRow, "resultDeyan"), cellValue, decimalFormat);
  cellValue = "=SUM(" + getRangeText(currentRow + 1, "resultRadostina", currentRow + 2, "resultRadostina") + ")";
  setCellValue(getRangeText(currentRow, "resultRadostina"), cellValue, decimalFormat);
  
  currentRow++;
  setCellValue(getRangeText(currentRow, "resultHeaders"), "Приходи", boldIf);
  cellValue = "=SUMIF(" + valueRange + ',">0")';
  setCellValue(getRangeText(currentRow, "resultTotal"), cellValue, decimalFormat);
  cellValue = "=SUMIF(" + deyanRange + ',">0")';
  setCellValue(getRangeText(currentRow, "resultDeyan"), cellValue, decimalFormat);
  cellValue = "=SUMIF(" + radiRange + ',">0")';
  setCellValue(getRangeText(currentRow, "resultRadostina"), cellValue, decimalFormat);
    
  currentRow++;
  var equalizationRow = tableStartRow + 9;
  setCellValue(getRangeText(currentRow, "resultHeaders"), "Разходи", boldIf);
  cellValue = "=SUMIF(" + valueRange + ',"<0")';
  setCellValue(getRangeText(currentRow, "resultTotal"), cellValue, decimalFormat);
  cellValue = "=SUMIF(" + deyanRange + ',"<0")';
  cellValue += (shouldAddCostsEqualizations ? ("+" + getRangeText(equalizationRow, "resultDeyan")) : "");
  setCellValue(getRangeText(currentRow, "resultDeyan"), cellValue, decimalFormat);
  cellValue = "=SUMIF(" + radiRange + ',"<0")';
  cellValue += (shouldAddCostsEqualizations ? ("+" + getRangeText(equalizationRow, "resultRadostina")) : "");
  setCellValue(getRangeText(currentRow, "resultRadostina"), cellValue, decimalFormat);
  
  currentRow+=2;
  var mutualExpensesRow = currentRow;
  setCellValue(getRangeText(currentRow, "resultHeaders"), reportConstants.types.mutualExpense);
  cellValue = "=SUMIF(" + typeRange + ", " + mutualExpense + ", " + valueRange + ')';
  setCellValue(getRangeText(currentRow, "resultTotal"), cellValue, decimalFormat);
  cellValue = "=SUMIF(" + typeRange + ", " + mutualExpense + ", " + deyanRange + ')';
  setCellValue(getRangeText(currentRow, "resultDeyan"), cellValue, decimalFormat);
  cellValue = "=SUMIF(" + typeRange + ", " + mutualExpense + ", " + radiRange + ')';
  setCellValue(getRangeText(currentRow, "resultRadostina"), cellValue, decimalFormat);
  
  currentRow++;
  setCellValue(getRangeText(currentRow, "resultHeaders"), reportConstants.types.personalExpense);
  cellValue = "=SUMIF(" + typeRange + ", " + personalExpense + ", " + valueRange + ')';
  setCellValue(getRangeText(currentRow, "resultTotal"), cellValue, decimalFormat);
  cellValue = "=SUMIF(" + typeRange + ", " + personalExpense + ", " + deyanRange + ')';
  setCellValue(getRangeText(currentRow, "resultDeyan"), cellValue, decimalFormat);
  cellValue = "=SUMIF(" + typeRange + ", " + personalExpense + ", " + radiRange + ')';
  setCellValue(getRangeText(currentRow, "resultRadostina"), cellValue, decimalFormat);
  
  currentRow+=2;
  var incomesRow = 3;
  var totalIncomesRange = getRangeText(incomesRow, "resultTotal", null, null, true);
  setCellValue(getRangeText(currentRow, "resultHeaders"), "Процент приход");  
  cellValue = "=" + totalIncomesRange + "/" + totalIncomesRange;
  setCellValue(getRangeText(currentRow, "resultTotal"), cellValue, percentFormat);
  cellValue = "=" + getRangeText(incomesRow, "resultDeyan", null, null, true) + "/" + totalIncomesRange;
  setCellValue(getRangeText(currentRow, "resultDeyan"), cellValue, percentFormat);
  cellValue = "=" + getRangeText(incomesRow, "resultRadostina", null, null, true) + "/" + totalIncomesRange;
  setCellValue(getRangeText(currentRow, "resultRadostina"), cellValue, percentFormat);
  
  currentRow++;
  var totalMutualExpensesRange = getRangeText(mutualExpensesRow, "resultTotal");
  setCellValue(getRangeText(currentRow, "resultHeaders"), "Процент общ разход");  
  cellValue = "=" + totalMutualExpensesRange + "/" + totalMutualExpensesRange;
  setCellValue(getRangeText(currentRow, "resultTotal"), cellValue, percentFormat);
  cellValue = "=" + getRangeText(mutualExpensesRow, "resultDeyan") + "/" + totalMutualExpensesRange;
  setCellValue(getRangeText(currentRow, "resultDeyan"), cellValue, percentFormat);
  cellValue = "=" + getRangeText(mutualExpensesRow, "resultRadostina") + "/" + totalMutualExpensesRange;
  setCellValue(getRangeText(currentRow, "resultRadostina"), cellValue, percentFormat);
  
  if(shouldAddCostsEqualizations){
    currentRow++;
    var totalAbsEnding = ")*ABS(" + totalMutualExpensesRange + ")";
    setCellValue(getRangeText(currentRow, "resultHeaders"), "Изравняване на разходи");  
    cellValue = "=(" + getRangeText(currentRow-1, "resultDeyan") + "-" + getRangeText(currentRow-2, "resultDeyan") + totalAbsEnding;
    setCellValue(getRangeText(currentRow, "resultDeyan"), cellValue, decimalFormat);
    cellValue = "=(" + getRangeText(currentRow-1, "resultRadostina") + "-" + getRangeText(currentRow-2, "resultRadostina") + totalAbsEnding;
    setCellValue(getRangeText(currentRow, "resultRadostina"), cellValue, decimalFormat);
  }  
};

function getWeekStatisticTableStart(weekIndex){
	var tableStart = parseInt(reportConstants.rows.start) + ((weekIndex + 1) * reportConstants.tablesRowOffset);
	
	return tableStart;
};

function generateWeekTable(week, weekIndex, valuesStart, valuesEnd){
	if (!shouldGenerateWeekReports) {
		return;
	}		
	
    var tableStart = getWeekStatisticTableStart(weekIndex);
    var tableHeader = week.start.getDate() + "-" + week.end.getDate() + " " + getMonthAndYear(week.start);
    
    generateStatisticTable({
      valuesStart: valuesStart,
      valuesEnd: valuesEnd,
      tableStartRow: tableStart,
      tableHeader: tableHeader,
      shouldAddCostsEqualizations: false,
    });
}

function useActiveSheet(){  
  reportSheet = SpreadsheetApp.getActiveSheet();
};

function setDataValidation(rangeText, valuesList){
	guardVariables();
	
	var cellRange = reportSheet.getRange(rangeText);
	cellRange.setDataValidation(null);
	cellRange.setDataValidation(SpreadsheetApp.newDataValidation().requireValueInList(valuesList).setAllowInvalid(false).build());
};

function addDataValidations(){	
	var ownerNameColumnRange = getRangeText(reportConstants.rows.start, reportConstants.columns.ownerName, reportConstants.rows.end, reportConstants.columns.ownerName);
	var ownerNamesList = [reportConstants.ownerNames.deyan, reportConstants.ownerNames.radostina];
	setDataValidation(ownerNameColumnRange, ownerNamesList);
	
	var typesColumnRange = getRangeText(reportConstants.rows.start, reportConstants.columns.type, reportConstants.rows.end, reportConstants.columns.type);
	var typesList = [reportConstants.types.mutualExpense, reportConstants.types.personalExpense, reportConstants.types.income];
	setDataValidation(typesColumnRange, typesList);	
};

function generateMonthReportSheet(dateText) {
  useActiveSheet();
  
  var cellValue = null;  
  var date = parseDate(dateText);     
  var bold = { fontWeight: "bold" };
  var bigBold = { fontWeight: "bold", fontSize: 12 };
  setCellValue(getRangeText("header", "value"), "Стойност в лв.", bold);
  setCellValue(getRangeText("header", "description"), "Описание на приход/разход", bold);
  setCellValue(getRangeText("header", "ownerName"), "Кой?", bold);
  setCellValue(getRangeText("header", "type"), "Вид приход/разход", bold);
  setCellValue(getRangeText("header", "deyanHelper"), reportConstants.ownerNames.deyan);
  setCellValue(getRangeText("header", "radostinaHelper"), reportConstants.ownerNames.radostina);
  setCellValue(getRangeText("header", "resultTotal"), "Общи резултати", bigBold);
  setCellValue(getRangeText("header", "resultDeyan"), reportConstants.ownerNames.deyan, bigBold);
  setCellValue(getRangeText("header", "resultRadostina"), reportConstants.ownerNames.radostina, bigBold);
  
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
    
  var valueBeforeValuesEnd = (parseInt(valuesEnd) - 1) + "";
  var weeks = getWeeksInMonth(date);
  
  for(var i = 0; i < weeks.length; i++){
    var week = weeks[i];    
    generateWeekTable(week, i, valuesStart, valuesEnd);    
    valuesStart = valueBeforeValuesEnd;
  }  
  
  var resizeColumns = reportConstants.columns.autoResizables;
  
  for(var i = 0; i < resizeColumns.length; i+=1){
    var columnIndex = resizeColumns[i];
    reportSheet.autoResizeColumn(columnIndex);
  }
  
  addDataValidations();  
  reportSheet.setFrozenRows(1);  
  reportSheet.setName(getMonthAndYear(date));
};

function getWeekAndWeekIndex(date){
  var weeks = getWeeksInMonth(date);
  var dateNum = date.getDate();
  
  for(var i = 0; i < weeks.length; i+=1){
    var week = weeks[i];    
    if(week.start.getDate() <= dateNum && dateNum <= week.end.getDate()){
      return  {
        weekIndex: i,
        week: week
      };
    }
  }
  
  throw {"message": "Calculations error! Cannot find week index!"};
};

function applyWeekStatisticsChanges(weekStatisticInfos){	
	useActiveSheet();
	
	for(var i = 0; i < weekStatisticInfos.length; i+=1){
		var weekInfo = weekStatisticInfos[i];
		
		if(weekInfo.initialValues.start != weekInfo.finalValues.start || weekInfo.initialValues.end != weekInfo.finalValues.end){
			var week = {
				start: parseDate(weekInfo.dates.start),
				end: parseDate(weekInfo.dates.end)
			};
			
			generateWeekTable(week, i, weekInfo.finalValues.start, weekInfo.finalValues.end);
		}
	}
};

function promptWeekStatisticChanges(monthDate){
	var weeks = getWeeksInMonth(monthDate);
	var minValue = reportConstants.rows.start;
	var maxValue = reportConstants.rows.end;
	var weekInfos = [];
	var html = '<table><tr><th>Седмица</th><th>От ред</th><th>До ред</th></tr>';
	
	for(var i = 0; i < weeks.length; i+=1){
		var week = weeks[i];
		var startRow = getWeekStatisticTableStart(i);
		var headerRange = getRangeText(startRow - 1, "resultHeaders");
		var weekHeaderValue = getDisplayValue(headerRange);
		var formulaRange = getRangeText(startRow + 1, "resultTotal");
		var formulaValue = getFormulaValue(formulaRange);
		var bracketIndex = formulaValue.indexOf("(");
		var rangeDelimiterIndex = formulaValue.indexOf(":");
		var commaIndex = formulaValue.indexOf(",");
		var isMultiCellsRange = (rangeDelimiterIndex > -1);
		var weekStartValue = formulaValue.substring(bracketIndex + 2, (isMultiCellsRange ? rangeDelimiterIndex : commaIndex));
		var weekEndValue = (isMultiCellsRange ? formulaValue.substring(rangeDelimiterIndex + 2, commaIndex) : weekStartValue);
		weekInfos[i] = {
			dates : {
				start : stringifyDate(week.start),
				end : stringifyDate(week.end)
			},
			initialValues : {
				start : parseInt(weekStartValue),
				end : parseInt(weekEndValue)
			},
			finalValues : { }
		};
		
		html += '<tr>';
		html += '<td>' + weekHeaderValue + '</td>';
		html += '<td><input type="number" id="fromRowWeek' + i + '" min="' + minValue + '" max="' + maxValue + '" value="' + weekStartValue + '" \></td>';
		html += '<td><input type="number" id="toRowWeek' + i + '" min="' + minValue + '" max="' + maxValue + '" value="' + weekEndValue + '" \></td>';
		html += '</tr>';
	}
	
	html += '</table>';
	
	html += '<p>'
	html += '<input type="button" value="Запази" onclick="parseWeekStatistics()" />';
	html += '&nbsp;&nbsp;&nbsp;';
	html += '<input type="button" value="Отмени" onclick="closeDialog()" />';
	html += '</p>';
	html += '<script src="//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>';
	html += '<script>';
	html += 'function parseWeekStatistics(){';
	html += 'var weekInfos = JSON.parse(' + "'" + JSON.stringify(weekInfos) + "'" + ');';
	for(var i = 0; i < weekInfos.length; i+= 1){
		html += 'weekInfos[' + i + '].finalValues.start = parseInt($("#fromRowWeek' + i + '").val());';
		html += 'weekInfos[' + i + '].finalValues.end = parseInt($("#toRowWeek' + i + '").val());';
	}
	html += 'google.script.run.withSuccessHandler(closeDialog).applyWeekStatisticsChanges(weekInfos);';
	html += '};';
	html += 'function closeDialog(){';
	html += 'google.script.host.close();';
	html += '};';
	html += '</script>';
	
	var htmlOutput = HtmlService
     .createHtmlOutput(html)
     .setWidth(330)
     .setHeight(250);
	SpreadsheetApp.getUi().showModalDialog(htmlOutput, "Настройки за седмични статистики");
};

function promptReportSheetMonthAndYear(){
	var currentDate = new Date();
	var year = currentDate.getFullYear();
	var monthIndex = currentDate.getMonth();	
	var day = currentDate.getDate();
	
	if (day > 20) {
		monthIndex++;

		if (monthIndex > 11) {
			monthIndex = 0;
			year++;
		}
	}
	
	var html = '<select id="monthSelect">';
	for (var i = 0; i < monthNames.length; i+=1){
		html += '<option value="' + (i + 1) + '"' + ((i == monthIndex) ? ' selected>' : '>') + monthNames[i] + '</option>';
	}
	
	html += '</select>';	
	html += '&nbsp;&nbsp;&nbsp;';
	html += '<input type="number" id="yearInput"' + ' min="' + 2000 + '" max="' + 2100 + '" value="' + year + '" \>';
	html += '<p>'
	html += '<input type="button" value="Генерирай" onclick="applyMonthReportSelections()" />';
	html += '&nbsp;&nbsp;&nbsp;';
	html += '<input type="button" value="Отмени" onclick="closeDialog()" />';
	html += '</p>';
	html += '<script src="//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>';
	html += '<script>';
	html += 'function applyMonthReportSelections(){';
	html += 'var selectedMonthNumber = $("#monthSelect").val();';
	html += 'var selectedYear = $("#yearInput").val();';
	html += 'var dateText = "1-" + selectedMonthNumber + "-" + selectedYear;';
	html += 'google.script.run.withSuccessHandler(closeDialog).generateMonthReportSheet(dateText);';
	html += '};';
	html += 'function closeDialog(){';
	html += 'google.script.host.close();';
	html += '};';
	html += '</script>';
	
	var htmlOutput = HtmlService
     .createHtmlOutput(html)
     .setWidth(280)
     .setHeight(90);
	SpreadsheetApp.getUi().showModalDialog(htmlOutput, "Избери месец и година за репорта");
};

function setupWeekStatistics(){
	useActiveSheet();
	
	var monthDate = tryGetCurrentReportMonthDate();	
	if(!monthDate){
		alert("Invalid sheet name - cannot parse current month info!");
		return;
	}
	
	promptWeekStatisticChanges(monthDate);		
};

function setupMonthReportSheet(){
	useActiveSheet();
	
	var shouldSetupMonthReportSheet = true;
	var monthDate = tryGetCurrentReportMonthDate();
	
	if(monthDate){
		var ui = SpreadsheetApp.getUi();
		var message = 'Текущият таб изглежда е вече генериран репорт. Генериране на нов репорт в същия таб би могло да изгуби инфомацията за седмични статистики. Сигурен ли си, че искаш да продължиш?';
		var response = ui.alert('Внимание!', message, ui.ButtonSet.YES_NO);
		shouldSetupMonthReportSheet = (response == ui.Button.YES);
	}
	
	if(shouldSetupMonthReportSheet){
		promptReportSheetMonthAndYear();
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
      name : "Генерирай месечен репорт",
      functionName : "setupMonthReportSheet"
    },
  ];
  
  if (shouldGenerateWeekReports) {
	entries[1] =
    {
      name : "Настрой седмични статистики",
      functionName : "setupWeekStatistics"
    };
  }
  
  spreadsheet.addMenu(reportConstants.appName, entries);
};

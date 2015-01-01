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

function generateSheet() {  
  var sheet = SpreadsheetApp.getActiveSheet();
  logObjectProperties(sheet);
  
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

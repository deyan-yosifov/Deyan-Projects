function placeHtmlTree(parent) {
    parent.innerHTML = getHtmlTree();
    attachTreeOnClickEvents('treeNode');
}

function getHtmlTree() {
    return '<div class="treeNode">\n            <span>Program</span>\n            <div class="treeNode closed">\n                <span>Monday</span>\n            </div>\n            <div class="treeNode closed">\n                <span>Tuesday</span>\n                <div class="treeNode closed">\n                    <span>Quality Programming Code</span>\n                    <div class="treeNode closed">\n                        <span>14-18</span>\n                    </div>\n                </div>\n            </div>\n            <div class="treeNode closed">\n                <span>Wednesday</span>\n                <div class="treeNode closed">\n                    <span>Slice And Dice</span>\n                    <div class="treeNode closed">\n                        <span>18-22</span>\n                    </div>\n                </div>\n                <div class="treeNode closed">\n                    <span>Business</span>\n                    <div class="treeNode closed">\n                        <span>19-22</span>\n                    </div>\n                </div>\n            </div>\n            <div class="treeNode closed">\n               <span>Thursday</span>\n                <div class="treeNode closed">\n                    <span>Javascript</span>\n                    <div class="treeNode closed">\n                        <span>14-18</span>\n                    </div>\n                </div>\n                <div class="treeNode closed">\n                    <span>CMS</span>\n                    <div class="treeNode closed">\n                        <span>19-22</span>\n                    </div>\n                </div>\n                <div class="treeNode closed">\n                    <span>OS</span>\n                    <div class="treeNode closed">\n                        <span>19-22</span>\n                    </div>\n                </div>\n            </div>\n            <div class="treeNode closed">\n                <span>Friday</span>\n                <div class="treeNode closed">\n                    <span>Quality Programming Code</span>\n                    <div class="treeNode closed">\n                        <span>14-18</span>\n                    </div>\n                </div>\n            </div>\n            <div class="treeNode closed">\n                <span>Suterday</span>\n            </div>\n            <div class="treeNode closed">\n                <span>Tuesday</span>\n                <div class="treeNode closed">\n                    <span>CMS</span>\n                    <div class="treeNode closed">\n                        <span>10-15</span>\n                    </div>\n                </div>\n            </div>\n        </div>';
}

function attachTreeOnClickEvents(className) {
    var elements = document.querySelectorAll("." + className);
    var count = elements.length;
    for (var i = 0; i < count; i++) {
        elements[i].onclick = addRemoveCloseClass;
    }
}

function addRemoveCloseClass(e) {
    var obj = e.currentTarget;    
    var className = "" + obj.className;
    if (className.indexOf("closed") > -1) {
        className = className.replace("closed","").trim();
    }
    else {
        className += " closed";
    }    
    obj.className = className;
    e.preventDefault();
    e.stopPropagation();
}

//TO DO
//NOT IMPLEMENTED YET, SO I USE getHtmlTree()
function generateTreeFromJson(parent, jsonTree, rootValue, treeNodeClassName) {
    if (treeNodeClassName === undefined) treeNodeClassName = 'treeNode';
    var div = document.createElement('div');
    var span = document.createElement('span');
    var fragment = document.createDocumentFragment();
    div.setAttribute("class", treeNodeClassName);

    var currentElement;
    var queue = [];
    queue.push(jsonTree);
	//TO DO BFS OR DFS
    
    parent.appendChild(fragment);
}

function getJsonTree() {
    var result = {
        Monday: {
        },
        Tuesday: { 
            QPC: "14-18" 
        },
        Wednesday: {
            SliceAndDice: "18-22",
            Business: "19-22"
        },
        Thursday: {
            Javascript: "14-18",
            CMS: "19-22",
            OS: "19-22"
        },
        Friday:{
            QPC: "14-18"
        },
        Suterday: {
        },
        Sunday: {
            CMS: "10-15"
        }
    };
    return result;
}
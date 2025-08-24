var curPage;
//For Selecting Model Dropdown that have equipment and model filters. Stores the model Id temporarily so that model list loads as
//result of equipment change and then the model is selected.
var selectedModel;

//Stores the History of pages visited by user.
var crumbHistory;

//Sets unique ID for each tab. If tabId is already assigned, uses that or creates a new one.
//var tabID = sessionStorage.tabID ? sessionStorage.tabID : sessionStorage.tabID = Math.random() * (100000000);

//
var tabID = sessionStorage.tabID && sessionStorage.closedLastTab !== '2' ? sessionStorage.tabID : sessionStorage.tabID = generateTabId();


//#region Unique Tab ID for IE
//Description: TabID was not unique in IE for this code handles the problem.

//When page is loaded from the same tab closedLastTab will be 1. otherwise it will be 2. The above line uses same tabID when closedLasttab is not 2.
sessionStorage.closedLastTab = '2';
$(window).on('unload beforeunload', function () {

    sessionStorage.closedLastTab = '1';
});

//#endregion


//Pages Json, Contains Page title and Ids.
var pages = [
    { Text: 'Dashboard', Id: 'Dashboard' },
    { Text: 'Manage Employees', Id: 'ManageEmployees' },
    { Text: 'Employee Details', Id: 'EmployeeDetails' },
    { Text: 'My Profile', Id: 'MyProfile' },
    { Text: 'Add Employees', Id: 'AddEmployees' },
    { Text: 'Manage Sites', Id: 'ManageSites' },
    { Text: 'Add Site', Id: 'AddSite' },
    { Text: 'Training Details', Id: 'TrainingDetails' },
    { Text: 'View Attendance', Id: 'ViewAttendance' },
    { Text: 'Apply Leaves', Id: 'ApplyLeaves' },
    { Text: 'Search Leaves', Id: 'SearchLeaves' },
    { Text: 'Leaves Policy Management', Id: 'LeavesPolicyManagement' },
    { Text: 'Approve/Reject Leaves', Id: 'ApproveRejectLeaves' },
    { Text: 'My Schedule', Id: 'MySchedule' },
    { Text: 'Manage Schedule', Id: 'ManageSchedule' },
    { Text: 'Manage Users', Id: 'ManageUsers' },
    { Text: 'Create Users', Id: 'CreateUsers' },
    { Text: 'Add Department', Id: 'AddDepartments' },
    { Text: 'Manage Departments', Id: 'ManageDepartments' },
    { Text: 'Create New Survey', Id: 'CreateNewSurvey' },
    { Text: 'Create Company News', Id: 'CreateNews' },
    { Text: 'Company News Feed', Id: 'CompanyNewsFeed' },
    { Text: 'Manage Company News Feed', Id: 'ManageCompanyNewsFeed' },
    { Text: 'Company News', Id: 'CompanyNews' },
    { Text: 'Manage Surveys', Id: 'ManageSurveys' },
    { Text: 'My Surveys', Id: 'MySurveys' },
    { Text: 'Employee Survey', Id: 'EmployeeSurvey' },
    { Text: 'Permisions', Id: 'ManagePermissions' },
    { Text: 'Company Reports', Id: 'CompanyReports' },
    { Text: 'Sites', Id: 'SiteReports' },
    { Text: 'Employee Gender', Id: 'GenderReports' },
    { Text: 'Employee Age', Id: 'AgeReports' },
    { Text: 'Departments', Id: 'DepartmentReports' },
    { Text: 'HeadCounts', Id: 'HeadCountsReports' },
    { Text: 'Hired Vs LeftReport', Id: 'HiredVsLeftReports' },
    { Text: 'BirthDay List', Id: 'BirthDayListReport' },
    { Text: 'Manage Positions', Id: 'ManagePositions' },
    { Text: 'Termination/Dismissal Reasons', Id: 'DismissalReasonReports' },
    { Text: 'Termination/Dismissal Types', Id: 'DismissalTypesReports' },
    { Text: 'Employee Levels', Id: 'EmployeeLevels' },
    { Text: 'Work Duration', Id: 'WorkDurationStats' }
];

//Clears Crumb History when user navigates from Side bar. e-g User goes to My Time from the Left Side bar.
$(document).on('click', '.app-sidebar .app-sidebar-menu .app-sidebar-wrapper .page-sidebar-menu .menu-item .menu-link > a', function () {
    startCrumb();
});

//Sets the BreadCrumbs HTML Dynamically based on history.
//Params: CurPage is the 'Id' of the Current opened page, CurPath is the URL of current page.
//Called on Document ready function inside this script.
function getPageCrumbs(curPage, curPath) {
    ////Clear Crumb history on selected pages OR if no history is present.
    var curPageArr = [
        'Dashboard',
        'ManageEmployees',
        'EmployeeDetails',
        'MyProfile',
        'AddEmployees',
        'ManageSites',
        'AddSite',
        'TrainingDetails',
        'ViewAttendance',
        'ApplyLeaves',
        'LeavesPolicyManagement',
        'SearchLeaves',
        'ApproveRejectLeaves',
        'MySchedule',
        'ManageSchedule',
        'ManageUsers',
        'CreateUsers',
        'AddDepartment',
        'ManageDepartments',
        'CreateNewSurvey',
        'CreateNews',
        'CompanyNewsFeed',
        'ManageCompanyNewsFeed',
        'CompanyNews',
        'ManageSurveys',
        'MySurveys',
        'EmployeeSurvey',
        'ManagePermissions',
        'CompanyReports',
        'SiteReports',
        'GenderReports',
        'AgeReports',
        'DepartmentReports',
        'HeadCountsReports',
        'HiredVsLeftReports',
        'BirthDayListReport',
        'ManagePositions',
        'DismissalReasonReports',
        'DismissalTypesReports',
        'EmployeeLevels',
        'WorkDurationStats',
        'Training'
    ];

    if (curPageArr.indexOf(curPage) > -1 || !JSON.parse(localStorage.getItem('BreadCrumbs' + tabID))) {

        //Starts new history with only home page inside it. Callback function is passed that is executed when new history is saved. It sets the global variables value to current history.
        startCrumb(function () {
            crumbHistory = JSON.parse(localStorage.getItem('BreadCrumbs' + tabID));
        });
    }
    else { // Getting history from local storage is history is already present and page is not first hierarchy page.
        crumbHistory = JSON.parse(localStorage.getItem('BreadCrumbs' + tabID));
    }

    //Variable to story new History.
    var newState = [];

    //Temp variable to know if Curpage is already present in History.
    var found = false;

    //Looping through old history to check if current page is already present.
    //Pushing old Items to new history.
    for (var i = 0; i < crumbHistory.length; i++) {
        newState.push(crumbHistory[i]);

        //In case user comes back using BreadCrumbs OR Back button.
        //If Curpage is already present ignore items after that. So the loop breaks.
        if (crumbHistory[i].Id === curPage) {
            found = true;
            break;
        }

    }

    //If Current page is not found in history it is pushed 
    if (!found) {

        //Getting Page Title and Id from pages Json.
        var nCrumb = pages.filter(function (crumb) {
            return crumb['Id'] == curPage;
        })[0];

        //Current path will be the URL for this crumb.
        nCrumb.Path = curPath;

        newState.push(nCrumb);
    }

    //New History will be set for further use.
    crumbHistory = newState;

    //Saving new history to local storage for later use.
    saveCrumbs(crumbHistory);

    //Making HTML for Bread Crumbs
    var crumbsHTML = '';

    //Looping through each item present in Crumbs History.
    for (var i = 0; i < crumbHistory.length; i++) {
        var c = crumbHistory[i];
        var crHtml = '';

        var homeHREF = getVirtualDirectory() + 'Home/Index';

        if (document.android) {
            homeHREF = 'javascript:AndroidFunction.backButtonNative();';
        } else if (document.desktop) {
            homeHREF = 'javascript:window.external.CloseOnlineWebView();';
        }

        //For home page. Adds Icon plus the URL is always same.
        if (i == 0) {

            crHtml = '<li class="breadcrumb-item"><a href="' + homeHREF + '" class="fw-bold fs-14px" style="color:#1F4293 !important"><i class="fa fa-home" style="color:#1F4293 !important"></i> &nbsp;' + "Home" + '</a></li>';
            crHtml += '<li class="breadcrumb-item"><i class="fa fa-chevron-right fs-11px" style="color:#1F4293 !important"></i></li>'
            // crHtml = '<li class="breadcrumb-item"><a href="' + homeHREF + '" class="fw-bold fs-14px" style="color:#1F4293 !important"><i class="fa fa-home" style="color:#1F4293 !important"></i> &nbsp;' + $.i18n("Home") + '</a></li>'

            // crHtml = '<li><a href="' + homeHREF + '"><i class="fa fa-home"></i>&nbsp' + $.i18n("Home") + '</a><i class="fa fa-chevron-right"></i></li>';
        }
        else
            if (i == crumbHistory.length - 1) { // If Item is last one. It won't be a Link.
                crHtml = '<li class="breadcrumb-item fs-14px fw-bold" style="color:#1F4293 !important" data-i18n="' + c.Text + '">' + c.Text + '</li>';
                // crHtml = '<li class="breadcrumb-item fs-14px fw-bold" style="color:#1F4293 !important" data-i18n="">' + $.i18n(c.Text) + '</li>';

                // crHtml = '<li><span>' + $.i18n(c.Text) + '</span></li>';
            }
            else { //For normal Items.
                crHtml = '<li class="breadcrumb-item"><a href="' + getURL(c.Path) + '" class="fw-bold fs-14px" style="color:#1F4293 !important"> &nbsp;' + c.Text + '</a></li>';
                // crHtml = '<li class="breadcrumb-item"><a href="' + getURL(c.Path) + '" class="fw-bold fs-14px" style="color:#1F4293 !important"> &nbsp;' + $.i18n(c.Text) + '</a></li>';
                crHtml += '<li class="breadcrumb-item"><i class="fa fa-chevron-right fs-11px" style="color:#1F4293 !important"></i></li>'


                // crHtml = '<li><a href="' + getURL(c.Path) + '">' + $.i18n(c.Text) + '</a><i class="fa fa-chevron-right"></i></li>';
            }
        //Appending HTML
        crumbsHTML = crumbsHTML + crHtml;
    }

    //Setting HTML to the breadcrumbs container div.
    $('.breadcrumb').html(crumbsHTML);

}

//Starts new history with only home page inside it. Callback function is passed that is executed when new history is saved. It sets the global variables value to current history.
function startCrumb(callback) {

    //Initializing new history.
    crumbHistory = new Array();

    //Push Home page as default start page for all hierarchies.
    crumbHistory.push({ Text: 'Home', Id: 'Home', Path: '~/Home/Index' });

    //save history to localstorage to retain in next page.
    saveCrumbs(crumbHistory);

    if (callback) {
        return callback();
    }
}

//Saves the crumb history to localStorage.
function saveCrumbs(hist) {

    ////Getting TabId.
    //tabID = sessionStorage.tabID ? sessionStorage.tabID : sessionStorage.tabID = Math.random();

    localStorage.setItem('BreadCrumbs' + tabID, JSON.stringify(hist));
    //console.log(crumbHistory);
}


//Ready Function that sets dynamic bread crumbs.
$(function () {
    // 'curPage' variable is set on each page that requires dynamic breadcrumbs.
    // All above logic only calls when page has this variable set. and that Id is also present on the pages json.
    // All other pages have static bread crumbs.
    if (curPage) {
        getPageCrumbs(curPage, location.href);
    }

});


//Saves the filters applied to the page into cookies.
function savePageState() {

    //Gets Name according to the page title to set the cookie.
    var cookieName = $('.page-title').text().trim().replace(/\s+/g, '') + '_PageFilters';

    //Empty object that will be filled with the filters data as properties.
    // Input Ids will be the key of the property and values will be the value of that property.
    var jsState = {};

    //Looping through each input and Select element in the filters. 
    //<<<<NOTE>>>>>> "sec_filters" class must be given to div encapsulating the filter inputs.
    $('.sec_filters').find('input,select').each(function (i, el) {
        ///Getting Input ID and Value
        var nm = $(el).attr('id');
        var val = $(el).val();
        ////Setting object property to current input's Id and value.
        jsState[nm] = val;
    });

    //// Saving object in cookie. Encoding the JSON to URI for security purpose.<<<<<< Must be decoded at the time of retreival>>>>>>>
    KTCookie.set(cookieName, encodeURIComponent(JSON.stringify(jsState)));

    //Getting current URL
    var url = parent.location.toString();
    //If State save is not currently enabled, enable it.
    if (!isSaveState(true)) {
        ///Appending StateSave Query param to the current URL.
        url = getURL(parent.location.toString());
    }

    ////Setting the new URL. <<<This will not reload the page>>>>
    window.history.replaceState({}, $('title').val(), url);

}

//Checks whether to apply Save State, if the URL has save state query param.
//If save state is false it also clears the current saved filters in cookies.
// donotClear param prevents it from clearing the cookie on calling this method.
function isSaveState(donotClear) {
    var s = getQueryParams()['SaveState'] == 'true';
    if (s) {
        return true
    }
    else if (donotClear) {
        return false;
    }
    else {
        //Removing Cookie
        var cookieName = $('.page-title').text().trim().replace(/\s+/g, '') + '_PageFilters';
        KTCookie.remove(cookieName);
        return false;
    }
}




function clearSaveState() {
    //Removing Cookie
    var cookieName = $('.page-title').text().trim().replace(/\s+/g, '') + '_PageFilters';
    KTCookies.remove(cookieName);
}


//Appends savestate query param to the URL.
function getURL(path) {
    if (path.indexOf('?') > 0) {
        return path + '&SaveState=true';
    }
    else {
        return path + '?SaveState=true';
    }
}

//Generates a random String with 6 characters 
function generateTabId() {
    var text = "";
    var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    for (var i = 0; i < 6; i++)
        text += possible.charAt(Math.floor(Math.random() * possible.length));

    return text;
}

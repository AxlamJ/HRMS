var Ajax = {
    userFormToken: "",
    init: function (options) {
        options = options || {};
        Ajax.userFormToken = options.formToken || "";
        if (!window.ShowError) {
            window.ShowError = function (msg, okCallback) {
                Swal.fire({
                    text: msg,
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Ok",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        okCallback();
                    }
                });
            }
        }
        if (!window.ShowInfo) {
            window.ShowInfo = function (msg, okCallback) {
                Swal.fire({
                    text: msg,
                    icon: "info",
                    buttonsStyling: false,
                    confirmButtonText: "Ok",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        okCallback();
                    }
                });
            }
        }
        if (!window.ConfirmationDialogue) {
            window.ConfirmationDialogue = function (msg, okCallback) {
                Swal.fire({
                    text: msg,
                    icon: "info",
                    buttonsStyling: false,
                    confirmButtonText: "Ok",
                    cancelButtonText: "Cancel",
                    customClass: {
                        confirmButton: "btn btn-primary",
                        cancelButton: "btn btn-secondary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        okCallback();
                    }
                    if (result.isDenied) {
                        if (options.onNo) {
                            options.onNo();
                        }
                    }
                });
            }
        }
    },
    enqueueSpinner: function () {
        if (Ajax.spinnerQueue.length < 1)
            spin();
        Ajax.spinnerQueue.push('spinner');
    },
    dequeueSpinner: function () {
        setTimeout(function () {
            Ajax.spinnerQueue.pop();
            if (Ajax.spinnerQueue.length < 1) {
                unspin(1);
            }
        }, 500);
    },
    spinnerQueue: [],
    get: function (url, successCallback, options) {
        const token = localStorage.getItem('jwtToken');
        options = options || {};
        $.ajax({
            url: url,
            type: "GET",
            data: options.data || null,
            headers: {
                'Authorization': `Bearer ${token}`
            },
            beforeSend: function () {
                Ajax.enqueueSpinner();
            },
            success: function (resp) {
                successCallback(resp);
            },
            error: function (xhr) {
                var errorMsg = /*$.i18n*/("An unknown error occurred. If the problem persists please contact your system administrator.");

                if (options.error) options.error(xhr.responseJSON);
                else if (xhr.status == 400) ShowError((xhr.responseJSON || {}).Message || errorMsg);
                else if (xhr.status == 401 && url.indexOf(sitePath) > -1) window.location.href = sitePath;
                else if (xhr.status == 403) ShowError(/*$.i18n*/("You are not allowed to access this information."));
                else if (xhr.status == 500) {
                    if (xhr.responseJSON.ErrorId && xhr.responseJSON.ErrorMessage) {
                        ShowError(xhr.responseJSON.ErrorMessage + xhr.responseJSON.ErrorId);
                    }
                    else {
                        ShowError(errorMsg + " " + ((xhr.responseJSON || {}).Message || ""));
                    }
                }
                else ShowError(errorMsg);
            },
            complete: function () {
                if (options.complete) {
                    options.complete();
                }
                options.complete && options.complete();
                Ajax.dequeueSpinner();
            }
        });
    },
    getNotifications: function (url, successCallback, options) {
        const token = localStorage.getItem('jwtToken');
        options = options || {};
        $.ajax({
            url: url,
            type: "GET",
            data: options.data || null,
            headers: {
                'Authorization': `Bearer ${token}`
            },
            beforeSend: function () {
                //Ajax.enqueueSpinner();
            },
            success: function (resp) {
                successCallback(resp);
            },
            error: function (xhr) {
                var errorMsg = /*$.i18n*/("An unknown error occurred. If the problem persists please contact your system administrator.");

                if (options.error) options.error(xhr.responseJSON);
                else if (xhr.status == 400) ShowError((xhr.responseJSON || {}).Message || errorMsg);
                else if (xhr.status == 401 && url.indexOf(sitePath) > -1) window.location.href = sitePath;
                else if (xhr.status == 403) ShowError(/*$.i18n*/("You are not allowed to access this information."));
                else if (xhr.status == 500) {
                    if (xhr.responseJSON.ErrorId && xhr.responseJSON.ErrorMessage) {
                        ShowError(xhr.responseJSON.ErrorMessage + xhr.responseJSON.ErrorId);
                    }
                    else {
                        ShowError(errorMsg + " " + ((xhr.responseJSON || {}).Message || ""));
                    }
                }
                else ShowError(errorMsg);
            },
            complete: function () {
                if (options.complete) {
                    options.complete();
                }
                options.complete && options.complete();
                //Ajax.dequeueSpinner();
            }
        });
    },
    dataTable: function (url, data, successCallback, options) {
        const token = localStorage.getItem('jwtToken');
        options = options || {};
        $.ajax({
            url: url,
            type: "GET",
            data: data,
            headers: {
                'Authorization': `Bearer ${token}`
            },
            //headers: { timeZoneOffset: new Date().getTimezoneOffset() },
            beforeSend: function () {
                Ajax.enqueueSpinner();
            },
            success: function (resp) {
                successCallback(resp);
            },
            error: function (xhr) {
                var errorMsg = /*$.i18n*/("An unknown error occurred. If the problem persists please contact your system administrator.");

                if (options.error) options.error(xhr.responseJSON);

                if (xhr.status == 400) ShowError((xhr.responseJSON || {}).Message || errorMsg);
                else if (xhr.status == 401 && url.indexOf(sitePath) > -1) window.location.href = sitePath;
                else if (xhr.status == 403) ShowError(/*$.i18n*/("You are not allowed to access this information."));
                else if (xhr.status == 500) {
                    if (xhr.responseJSON.ErrorId && xhr.responseJSON.ErrorMessage) {
                        ShowError(xhr.responseJSON.ErrorMessage + xhr.responseJSON.ErrorId);
                    }
                    else {
                        ShowError(errorMsg + " " + ((xhr.responseJSON || {}).Message || ""));
                    }
                }
                else ShowError(errorMsg);

            },
            complete: function () {
                if (options.complete) {
                    options.complete();
                }
                Ajax.dequeueSpinner();
            }
        });
    },
    post: function (url, data, successCallback, options) {
        const token = localStorage.getItem('jwtToken');
        options = options || {};
        $.ajax({
            url: url,
            type: "POST",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data),
            headers: {
                'Authorization': `Bearer ${token}`
            },
            //headers: { timeZoneOffset: new Date().getTimezoneOffset()/*, userFormToken: Ajax.userFormToken*/ },
            beforeSend: function () {
                if (!options.avoidSpinner) {
                    Ajax.enqueueSpinner();
                }
            },
            success: function (resp) {
                if (options.infiniteSpinner) {
                    Ajax.spinnerQueue.push('spinner');
                }
                successCallback(resp);
            },
            error: function (xhr) {
                var errorMsg = /*$.i18n*/("An unknown error occurred. If the problem persists please contact your system administrator.");

                if (options.error) options.error(xhr.responseJSON);
                else if (xhr.status == 400) ShowError((xhr.responseJSON || {}).Message || errorMsg);
                else if (xhr.status == 401 && url.indexOf(sitePath) > -1) window.location.href = sitePath;
                else if (xhr.status == 403) ShowError(/*$.i18n*/("You are not allowed to access this information."));
                else if (xhr.status == 500) {
                    if (xhr.responseJSON.ErrorId && xhr.responseJSON.ErrorMessage) {
                        ShowError(xhr.responseJSON.ErrorMessage + xhr.responseJSON.ErrorId);
                    }
                    else {
                        ShowError(errorMsg + " " + ((xhr.responseJSON || {}).Message || ""));
                    }
                }
                else if (xhr.status == 409) {
                    ShowError(xhr.responseJSON.Message);
                }
                else ShowError(errorMsg);
            },
            complete: function () {
                if (options.complete) {
                    options.complete();
                }
                if (!options.avoidSpinner) {
                    Ajax.dequeueSpinner();
                }
            }
        });
    },
    authenticate: function (url, data, successCallback, options) {
        const token = localStorage.getItem('jwtToken');
        options = options || {};
        $.ajax({
            url: url,
            type: "POST",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data),
            headers: {
                'Authorization': `Bearer ${token}`
            },
            //headers: { timeZoneOffset: new Date().getTimezoneOffset()/*, userFormToken: Ajax.userFormToken*/ },
            beforeSend: function () {
                if (!options.avoidSpinner) {
                    Ajax.enqueueSpinner();
                }
            },
            success: function (resp) {
                if (options.infiniteSpinner) {
                    Ajax.spinnerQueue.push('spinner');
                }
                successCallback(resp);
            },
            error: function (xhr) {
                var errorMsg = /*$.i18n*/("An unknown error occurred. If the problem persists please contact your system administrator.");

                if (options.error) options.error(xhr.responseJSON);
                else if (xhr.status == 400) ShowError((xhr.responseJSON || {}).Message || errorMsg);
                else if (xhr.status == 401 && url.indexOf(sitePath) > -1) ShowError((xhr.responseJSON || {}).Message || errorMsg);
                else if (xhr.status == 403) ShowError(/*$.i18n*/("You are not allowed to access this information."));
                else if (xhr.status == 500) {
                    if (xhr.responseJSON.ErrorId && xhr.responseJSON.ErrorMessage) {
                        ShowError(xhr.responseJSON.ErrorMessage + xhr.responseJSON.ErrorId);
                    }
                    else {
                        ShowError(errorMsg + " " + ((xhr.responseJSON || {}).Message || ""));
                    }
                }
                else if (xhr.status == 409) {
                    ShowError(xhr.responseJSON.ErrorMessage);
                }
                else ShowError(errorMsg);
            },
            complete: function () {
                if (options.complete) {
                    options.complete();
                }
                if (!options.avoidSpinner) {
                    Ajax.dequeueSpinner();
                }
            }
        });
    },
    put: function (url, data, successCallback, options) {
        const token = localStorage.getItem('jwtToken');
        options = options || {};
        $.ajax({
            url: url,
            type: "PUT",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data),
            headers: {
                'Authorization': `Bearer ${token}`
            },
            //headers: { timeZoneOffset: new Date().getTimezoneOffset()/*, userFormToken: Ajax.userFormToken*/},
            beforeSend: function () {
                if (!options.avoidSpinner) {
                    Ajax.enqueueSpinner();
                }
            },
            success: function (resp) {
                if (options.infiniteSpinner) {
                    Ajax.spinnerQueue.push('spinner');
                }
                successCallback(resp);
            },
            error: function (xhr) {
                //var errorMsg = $.i18n("An unknown error occurred. If the problem persists please contact your system administrator.");
                var errorMsg = /*$.i18n*/("An unknown error occurred. If the problem persists please contact your system administrator.");

                if (options.error) options.error(xhr.responseJSON);
                else if (xhr.status == 400) ShowError((xhr.responseJSON || {}).Message || errorMsg);
                else if (xhr.status == 401 && url.indexOf(sitePath) > -1) window.location.href = sitePath;
                else if (xhr.status == 403) ShowError(/*$.i18n*/("You are not allowed to access this information."));
                else if (xhr.status == 500) {
                    if (xhr.responseJSON.ErrorId && xhr.responseJSON.ErrorMessage) {
                        ShowError(xhr.responseJSON.ErrorMessage + xhr.responseJSON.ErrorId);
                    }
                    else {
                        ShowError(errorMsg + " " + ((xhr.responseJSON || {}).Message || ""));
                    }
                }
                else ShowError(errorMsg);
            },
            complete: function () {
                if (options.complete) {
                    options.complete();
                }
                if (!options.avoidSpinner) {
                    Ajax.dequeueSpinner();
                }
            }
        });
    },

    delete: function (url, data, successCallback, options) {
        const token = localStorage.getItem('jwtToken');
        options = options || {};
        $.ajax({
            url: url,
            type: "DELETE",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data),
            headers: {
                'Authorization': `Bearer ${token}`
            },
            //headers: { timeZoneOffset: new Date().getTimezoneOffset()/*, userFormToken: Ajax.userFormToken*/},
            beforeSend: function () {
                if (!options.avoidSpinner) {
                    Ajax.enqueueSpinner();
                }
            },
            success: function (resp) {
                if (options.infiniteSpinner) {
                    Ajax.spinnerQueue.push('spinner');
                }
                successCallback(resp);
            },
            error: function (xhr) {
                var errorMsg = /*$.i18n*/("An unknown error occurred. If the problem persists please contact your system administrator.");

                if (options.error) options.error(xhr.responseJSON);
                else if (xhr.status == 400) ShowError((xhr.responseJSON || {}).Message || errorMsg);
                else if (xhr.status == 401 && url.indexOf(sitePath) > -1) window.location.href = sitePath;
                else if (xhr.status == 403) ShowError(/*$.i18n*/("You are not allowed to access this information."));
                else if (xhr.status == 500) {
                    if (xhr.responseJSON.ErrorId && xhr.responseJSON.ErrorMessage) {
                        ShowError(xhr.responseJSON.ErrorMessage + xhr.responseJSON.ErrorId);
                    }
                    else {
                        ShowError(errorMsg + " " + ((xhr.responseJSON || {}).Message || ""));
                    }
                }
                else ShowError(errorMsg);
            },
            complete: function () {
                if (options.complete) {
                    options.complete();
                }
                if (!options.avoidSpinner) {
                    Ajax.dequeueSpinner();
                }
            }
        });
    }

};

HRMSUtil.onDOMContentLoaded(function () {
    if (!window.ShowError) {
        window.ShowError = function (msg, okCallback) {
            Swal.fire({
                text: msg,
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Ok",
                customClass: {
                    confirmButton: "btn btn-primary"
                }
            }).then(function (result) {
                if (result.isConfirmed) {
                    okCallback();
                }
            });
        }
    }
    if (!window.ShowInfo) {
        window.ShowInfo = function (msg, okCallback) {
            Swal.fire({
                text: msg,
                icon: "info",
                buttonsStyling: false,
                confirmButtonText: "Ok",
                customClass: {
                    confirmButton: "btn btn-primary"
                }
            }).then(function (result) {
                if (result.isConfirmed) {
                    okCallback();
                }
            });
        }
    }
    if (!window.ConfirmationDialogue) {
        window.ConfirmationDialogue = function (msg, okCallback) {
            Swal.fire({
                text: msg,
                icon: "info",
                buttonsStyling: false,
                confirmButtonText: "Ok",
                cancelButtonText: "Cancel",
                customClass: {
                    confirmButton: "btn btn-primary",
                    cancelButton: "btn btn-secondary"
                }
            }).then(function (result) {
                if (result.isConfirmed) {
                    okCallback();
                }
                if (result.isDenied) {
                    if (options.onNo) {
                        options.onNo();
                    }
                }
            });
        }
    }

});
$(document).on("click", "#btn-add-new-role", function () {
    $('#modal-edit-role').modal('show');
});

$(document).on("click", ".btn-edit-role", function () {

    var $self = $(this);
    var roleId = $self.data("id");

    Ajax.get(sitePath + "api/PermissionsAPI/GetRoleById?RoleId=" + roleId, function (resp) {
        if (resp.StatusCode == 200) {

            var roleData = resp.RoleData

            var role_info_form = document.querySelector("#role-info-form");
            var role_resources_form = document.querySelector("#resources-form");
            $(role_info_form).find('#role-id').val(roleData.RoleId);
            $(role_info_form).find('#role-name').val(roleData.RoleName);
            $(role_info_form).find('#role-description').val(roleData.Description);
            var RoleResources = JSON.parse(roleData.RoleResources);
            initializeSelections(RoleResources);
        }
        else {
            Swal.fire({
                text: "Error occured. Please contact your system administrator.",
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Ok",
                allowEscapeKey: false,
                allowOutsideClick: false,
                customClass: {
                    confirmButton: "btn btn-primary",
                },
                didOpen: function () {
                    hideSpinner();
                }
            });
        }

    });


    //if (EmployeeBio != null) {

    //    $(form).find("#about").text(EmployeeBio.About);
    //    $(form).find("#hobbies").text(EmployeeBio.Hobbies);
    //    $(form).find("#favorite-books").text(EmployeeBio.FavoriteBooks);
    //    $(form).find("#music-preference").text(EmployeeBio.MusicPreference);
    //    $(form).find("#sports").text(EmployeeBio.Sports);
    //}
    $('#modal-edit-role').modal('show');

});


$("#modal-edit-role").on('hide.bs.modal', function () {
    var role_info_form = document.querySelector("#role-info-form");
    var role_resources_form = document.querySelector("#resources-form");

    role_info_form.reset();
    role_resources_form.reset();
});


let selectedOptions = {
    "settings": ["Company account", "Employees"],
    "permissions": ["Admin Access"]
};


$(document).on('click', '.toggle-btn', function (event) {

    var $self = $(this);
    event.stopPropagation(); // Prevents parent checkbox from being triggered
    var sectionId = $self.data("section-id");
    let section = document.getElementById(sectionId);
    //$(section).style.display = $(section).style.display === "none" ? "block" : "none";
    //let sectionId = $(this).closest('.section').find('.child-container').attr('id');
    //let section = $('#' + sectionId);

    // Toggle the section visibility
    //section.toggle();


    //$self.stopPropagation();

    section.style.display = section.style.display === "none" ? "block" : "none";
    $self.find('i').toggleClass('fa-chevron-down fa-chevron-up');
})
//function toggleSection(event, sectionId) {
//    event.stopPropagation(); // Prevents checkbox selection
//    let section = document.getElementById(sectionId);
//    let toggleIcon = event.target;
//    section.style.display = section.style.display === "none" ? "block" : "none";
//    //toggleIcon = section.style.display === "block" ? '<i class="fa fa-chevron-up"></i>' : '<i class="fa fa-chevron-down"></i>';

//    //$(toggleIcon).toggleClass('fa-chevron-down fa-chevron-up');

//}

function initializeSelections(selectedOptions) {
    if (!selectedOptions || selectedOptions.length === 0) return; // Ensure data exists

    let selectedData = selectedOptions[0]; // Assuming there's only one object in the array

    Object.keys(selectedData).forEach(section => {
        let parentCheckbox = document.querySelector(`.parentCheckbox[data-section="${section}"]`);
        let childCheckboxes = document.querySelectorAll(`.child[data-section="${section}"]`);

        childCheckboxes.forEach(child => {
            if (selectedData[section].includes(child.value)) {
                child.checked = true;
            }
        });

        // If all children are checked, check the parent
        if (parentCheckbox) {
            parentCheckbox.checked = [...childCheckboxes].length > 0 && [...childCheckboxes].every(child => child.checked);
        }
    });
}

// Parent Checkbox Controls All Children
document.querySelectorAll(".parentCheckbox").forEach(parent => {
    parent.addEventListener("change", function () {
        let section = this.dataset.section;
        let children = document.querySelectorAll(`.child[data-section="${section}"]`);
        children.forEach(child => child.checked = this.checked);
    });
});

// Child Checkbox Controls Parent State
document.querySelectorAll(".child").forEach(child => {
    child.addEventListener("change", function () {
        let section = this.dataset.section;
        let parent = document.querySelector(`.parentCheckbox[data-section="${section}"]`);
        let children = document.querySelectorAll(`.child[data-section="${section}"]`);
        parent.checked = [...children].every(c => c.checked);
    });
});

// These are the constraints used to validate the form
var roleinfoconstraints = {
    "role-name": {
        // Site Name is required
        presence: {
            message: "is required."
        }
    },
    "role-description": {
        // Site Country is required
        presence: {
            message: "is required."
        }
    }

};


$(document).on('click', '#btn-update-role-details', function () {

    var form = document.querySelector("#role-info-form");
    var errors = validate(form, roleinfoconstraints);
    showErrors(form, errors || {});
    if (!errors) {
        var roleInfo = {};

        roleInfo.roleId = ($(form).find("#role-id").val() == null || $(form).find("#role-id").val() == "") ? null : $(form).find("#role-id").val();
        roleInfo.roleName = $(form).find("#role-name").val();
        roleInfo.description = $(form).find("#role-description").val();

        let roleResources = {};
        document.querySelectorAll(".parentCheckbox").forEach(parent => {
            let section = parent.dataset.section;
            let selectedChildren = [];
            document.querySelectorAll(`.child[data-section="${section}"]:checked`).forEach(child => {
                selectedChildren.push(child.value);
            });
            if (selectedChildren.length > 0) {
                roleResources[section] = selectedChildren;
            }
        });

        var roleResourcesArray = [];
        if (roleResources || Object.keys(roleResources).length > 0) {
            roleResourcesArray.push(roleResources)
        }

        roleInfo.roleResources = JSON.stringify(roleResourcesArray);

        if (Window.JustConsole) { console.log(roleInfo); return; }
        var url = sitePath + 'api/PermissionsAPI/UpsertRoleDetails';

        var message = roleInfo.roleId == null ? "New Role added successfully." : "Role updated successfully.";
        Ajax.post(url, roleInfo, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: message,
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Ok",
                    allowEscapeKey: false,
                    allowOutsideClick: false,
                    customClass: {
                        confirmButton: "btn btn-primary",
                    },
                    didOpen: function () {
                        hideSpinner();

                    }
                }).then(function (result) {
                    if (result.isConfirmed) {

                        window.location.reload();
                    }
                });
            }
            else {
                Swal.fire({
                    text: "Error occured. Please contact your system administrator.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Ok",
                    allowEscapeKey: false,
                    allowOutsideClick: false,
                    customClass: {
                        confirmButton: "btn btn-primary",
                    },
                    didOpen: function () {
                        hideSpinner();
                    }
                });
            }
        });
    }
    else {
        Swal.fire({
            text: "Please complete mandatory fields.",
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: "Ok",
            allowEscapeKey: false,
            allowOutsideClick: false,
            customClass: {
                confirmButton: "btn btn-primary",
            },
            didOpen: function () {
                hideSpinner();
            }
        });
    }
})
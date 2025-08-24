let currentPage = 1;
const pageSize = 12; // News per page


HRMSUtil.onDOMContentLoaded(function () {

    //PopulateDropDowns(function () {

    RenderListData(currentPage);
    //});
    hideSpinner();
});


function PopulateDropDowns(cb) {


    var ddDepartments = document.querySelector('#departments');
    $(ddDepartments).empty();
    var lstDepartments = (DropDownsData.Departments || []);
    var option = new Option();
    ddDepartments.appendChild(option);
    var option = new Option("All", -1, false, false);
    ddDepartments.appendChild(option);
    lstDepartments.forEach(function (item) {

        var option = new Option(item.DepartmentName, item.DepartmentId, false, false);
        ddDepartments.appendChild(option);
    });


    var ddSites = document.querySelector('#sites');
    $(ddSites).empty();
    var lstSites = (DropDownsData.Sites || []);
    var option = new Option();
    ddSites.appendChild(option);
    var option = new Option("All", -1, false, false);
    ddSites.appendChild(option);
    lstSites.forEach(function (item) {

        var option = new Option(item.SiteName, item.Id, false, false);
        ddSites.appendChild(option);
    });


    var ddEmployees = document.querySelector('#sites');
    $(ddEmployees).empty();
    var lstEmployees = (DropDownsData.Employees || []);
    var option = new Option();
    ddEmployees.appendChild(option);
    var option = new Option("All", -1, false, false);
    ddEmployees.appendChild(option);
    lstEmployees.forEach(function (item) {

        var option = new Option(item.FirtstName + " " + item.LastName, item.EmployeeCode, false, false);
        ddEmployees.appendChild(option);
    });

    if (cb) {
        cb();
    }


}

function RenderListData(pageNumber) {

    var queryData = {};

    queryData.PageNumber = pageNumber - 1;
    queryData.PageSize = pageSize;
    var url = sitePath + 'api/NewsFeedAPI/GetNewsFeed';

    Ajax.post(url, queryData, function (response) { // Ensure you use the correct data (exportRequestBody)
        //hideSpinner();

        if (response.StatusCode == 200) {
            renderNews(response.NewsList, function () {
                renderPagination(response.TotalRecords, pageNumber);
            })
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


function renderNews(News, cb) {
    const container = $("#newslist");
    container.empty();

    News.forEach(function (feed) {
        if (feed.VisibleTo == 'all') {
            var photoUrl = (feed.ImageUrl == "" || feed.ImageUrl == null) ? "/Images/no-image-1.svg" : feed.ImageUrl;
            const card = `
                    <div class="col-md-4 mb-4">
                    	<div class="card card-flush mb-10">
							<div class="card-body text-center">
                            	<div class="d-flex h-300px mh-300px text-center">
								    <img src="${photoUrl}" class="" alt="" style="width: -webkit-fill-available;" />
								</div>
                            	<div class="d-flex align-items-center mt-5">
									<div class="flex-grow-1">
										<h5 class="card-title">${feed.NewsTitle}</h5>
									</div>
								</div>
							</div>
							<div class="card-footer pt-0">
								<div class="mb-6">
                                    <div class="row">
                                        <div class="d-flex justify-content-center text-end gap-2">
                                            <a  href="${sitePath}NewsFeed/News?Id=${feed.Id}" class="btn btn-primary btn-lg" id="btn-read-more">
                                                Read More
                                            </a>
                                        </div>
                                    </div>
                                </div>
							</div>
						</div>
                    </div>`;
            container.append(card);

        }
        else if (feed.VisibleTo == "departments") {
            var departments = JSON.parse(feed.Departments);

            const departmentids = UserDepartments.map(function (dept) {
                return dept.id;
            });
            var hasMatch = departments.some(function (deptid) {
                return departmentids.includes(parseInt(deptid));
            });
            if (hasMatch) {

                var departmentssubcat = JSON.parse(feed.DepartmentsSubCategories);

                const departmentsubcatids = UserDepartmentSubCategories.map(function (subcat) {
                    return subcat.id;
                });
                var hassubCatMatch = departmentssubcat?.some(function (subcatid) {
                    return departmentsubcatids.includes(parseInt(subcatid));
                });
                if (departmentssubcat == null) {
                    hassubCatMatch = true;
                }

                if (hassubCatMatch) {
                    var photoUrl = (feed.ImageUrl == "" || feed.ImageUrl == null) ? "/Images/no-image-1.svg" : feed.ImageUrl;
                    const card = `
                    <div class="col-md-4 mb-4">
                    	<div class="card card-flush mb-10">
							<div class="card-body text-center">
                            	<div class="d-flex h-300px mh-300px text-center">
								    <img src="${photoUrl}" class="" alt="" style="width: -webkit-fill-available;" />
								</div>
                            	<div class="d-flex align-items-center mt-5">
									<div class="flex-grow-1">
										<h5 class="card-title">${feed.NewsTitle}</h5>
									</div>
								</div>
							</div>
							<div class="card-footer pt-0">
								<div class="mb-6">
                                    <div class="row">
                                        <div class="d-flex justify-content-center text-end gap-2">
                                            <a  href="${sitePath}NewsFeed/News?Id=${feed.Id}" class="btn btn-primary btn-lg" id="btn-read-more">
                                                Read More
                                            </a>
                                        </div>
                                    </div>
                                </div>
							</div>
						</div>
                    </div>`;
                    container.append(card);

                }
            }
        }
        else if (feed.VisibleTo == "sites") {
            var sites = JSON.parse(feed.Sites);
            const siteids = UserSites.map(function (site) {
                return site.id;
            });
            var hasMatch = sites.some(function (siteId) {
                return siteids.includes(parseInt(siteId));
            });


            if (hasMatch) {
                var photoUrl = (feed.ImageUrl == "" || feed.ImageUrl == null) ? "/Images/no-image-1.svg" : feed.ImageUrl;
                const card = `
                    <div class="col-md-4 mb-4">
                    	<div class="card card-flush mb-10">
							<div class="card-body text-center">
                            	<div class="d-flex h-300px mh-300px text-center">
								    <img src="${photoUrl}" class="" alt="" style="width: -webkit-fill-available;" />
								</div>
                            	<div class="d-flex align-items-center mt-5">
									<div class="flex-grow-1">
										<h5 class="card-title">${feed.NewsTitle}</h5>
									</div>
								</div>
							</div>
							<div class="card-footer pt-0">
								<div class="mb-6">
                                    <div class="row">
                                        <div class="d-flex justify-content-center text-end gap-2">
                                            <a  href="${sitePath}NewsFeed/News?Id=${feed.Id}" class="btn btn-primary btn-lg" id="btn-read-more">
                                                Read More
                                            </a>
                                        </div>
                                    </div>
                                </div>
							</div>
						</div>
                    </div>`;
                container.append(card);
            }
        }
        else if (feed.VisibleTo == "employees") {
            var employees = JSON.parse(feed.Employees);

            if (employees.indexOf(EmployeeCode) > -1) {
                var photoUrl = (feed.ImageUrl == "" || feed.ImageUrl == null) ? "/Images/no-image-1.svg" : feed.ImageUrl;
                const card = `
                    <div class="col-md-4 mb-4">
                    	<div class="card card-flush mb-10">
							<div class="card-body text-center">
                            	<div class="d-flex h-300px mh-300px text-center">
								    <img src="${photoUrl}" class="" alt="" style="width: -webkit-fill-available;" />
								</div>
                            	<div class="d-flex align-items-center mt-5">
									<div class="flex-grow-1">
										<h5 class="card-title">${feed.NewsTitle}</h5>
									</div>
								</div>
							</div>
							<div class="card-footer pt-0">
								<div class="mb-6">
                                    <div class="row">
                                        <div class="d-flex justify-content-center text-end gap-2">
                                            <a  href="${sitePath}NewsFeed/News?Id=${feed.Id}" class="btn btn-primary btn-lg" id="btn-read-more">
                                                Read More
                                            </a>
                                        </div>
                                    </div>
                                </div>
							</div>
						</div>
                    </div>`;
                container.append(card);
            }
        }

    });

    if (cb) {
        cb()
    }
}

function renderPagination(totalRecords, currentPage) {
    const totalPages = Math.ceil(totalRecords / pageSize);
    const pagination = $("#pagination");
    pagination.empty();

    const maxVisiblePages = 4;
    let startPage = Math.max(1, currentPage - Math.floor(maxVisiblePages / 2));
    let endPage = startPage + maxVisiblePages - 1;

    if (endPage > totalPages) {
        endPage = totalPages;
        startPage = Math.max(1, endPage - maxVisiblePages + 1);
    }

    // Previous Button
    pagination.append(`
            <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
                <a class="page-link btn btn-outline btn-outline-dashed border-black" href="#" onclick="RenderListData(${currentPage - 1})"><i class="fa fa-chevron-left fs-4"></i></a>
            </li>
        `);

    // Page Numbers
    for (let i = startPage; i <= endPage; i++) {
        pagination.append(`
                <li class="page-item ${i === currentPage ? 'active' : ''}">
                    <a class="page-link btn btn-outline btn-outline-dashed border-black" href="#" onclick="RenderListData(${i})">${i}</a>
                </li>
            `);
    }

    // Next Button
    pagination.append(`
            <li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
                <a class="page-link btn btn-outline btn-outline-dashed border-black" href="#" onclick="RenderListData(${currentPage + 1})"><i class="fa fa-chevron-right fs-4"></i></a>
            </li>
        `);
}


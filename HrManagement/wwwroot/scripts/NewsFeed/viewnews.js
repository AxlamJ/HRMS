
var NewsId = getParameterByName("Id") || null;

HRMSUtil.onDOMContentLoaded(function () {

    //PopulateDropDowns(function () {

    RenderNewsData();
    //});
    hideSpinner();
});


function RenderNewsData() {

    var queryData = {};

    var url = sitePath + 'api/NewsFeedAPI/GetNewsbyId?Id=' + NewsId;

    Ajax.get(url, function (response) { // Ensure you use the correct data (exportRequestBody)
        //hideSpinner();

        if (response.StatusCode == 200) {
            renderNews(response.News)
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


function renderNews(News) {
    const container = $("#newslist");
    container.empty();

    var photoUrl = (News.ImageUrl == "" || News.ImageUrl == null) ? "/Images/no-image-1.svg" : News.ImageUrl;
    var IsphotoUrl = (News.ImageUrl == "" || News.ImageUrl == null) ? "d-none" : "d-show";
    var youtubeUrl = (News.YoutubeUrl == "" || News.YoutubeUrl == null) ? "" : News.YoutubeUrl;
    var IsyoutubeUrl = (News.YoutubeUrl == "" || News.YoutubeUrl == null) ? "d-none" : "d-show";
    const card = `
                    <div class="col-md-12 mb-4">
                    	<div class="card card-flush mb-10">
                            <div class="card-header">
								<h5 class="card-title bolder">${News.NewsTitle}</h5>
                            </div>
							<div class="card-body text-center">
                            	<div class="d-flex mh-1000px  text-center ${IsphotoUrl}">
								    <img src="${photoUrl}" class="" alt="" style="width: -webkit-fill-available;"  />
								</div>
                            	<div class="d-flex justify-content-center text-center video-container ${IsyoutubeUrl} mt-10">
								     <iframe  style="width: 60vw;max-height: 70vh;"  
                                        src="${youtubeUrl}" 
                                        frameborder="0" 
                                        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" 
                                        allowfullscreen>
                                    </iframe>
								</div>
                            	<div class="d-flex align-items-center mt-5">
									<div class="flex-grow-1">
										<h5 class="card-title">${News.NewsTitle}</h5>
									</div>
								</div>
                                <div class="d-flex align-items-center mt-5">
									<div class="flex-grow-1">
										<p class="card-description">${News.Content}</p>
									</div>
								</div>
							</div>
						</div>
                    </div>`;
    container.append(card);

}
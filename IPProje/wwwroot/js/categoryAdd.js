$(document).ready(function () {
    $("#btnKaydet").click(function (event) {
        event.preventDefault();

        var addUrl = app.Urls.categoryAddUrl;
        var redirectUrl = app.Urls.categoryIndexUrl;

        var categoryAddModel = {
            Name: $("input[id=categoryName]").val()
        };

        var jsonData = JSON.stringify(categoryAddModel);
        console.log(jsonData);

        $.ajax({
            url: addUrl,
            type: "Post",
            contentType: "application/json; charset=utf-8",
            dataType: "Json",
            data: jsonData,
            success: function (data) {
                setTimeout(function () {
                    window.location.href = redirectUrl;
                }, 300);
            },
            error: function () {
                toast.error("Hata oluþtu.", "Hata");
            }
        });
    });
});
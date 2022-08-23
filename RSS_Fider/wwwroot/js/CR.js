function getInfo() {

    $.post("/RSS/RssIndex")
        .done(function (data) {

            var count = data.length;
            console.log(count);


            for (var i = 0; i < data.length; i++) {
                results.append('<li>' + data[i].Title + '</li>'); // добавляем данные в список
            }


            $("#result_text").html(result_str);
            console.log(data);
        });
}
/*
function updateModel() {
    $.ajax({
        url: "/RSS/RssIndex",
        type: "GET",
        dataType: "json",
        success: function (response) {

            if (response.data.length == 0) {
                // EMPTY
            } else {

                var obj = jQuery.parseJSON(response.data);
                console.log(obj);
            }
        }
    });
}*/




/*
<script type="text/javascript">
    function updateModelData() {
        setInterval(updateModel, 3000);
    var i = 0;
    function updateModel() {
        $.ajax({
            url: "/RSS/RSSRefresh",
            type: "GET",
            dataType: "json",
            success: function (response) {

                if (response.data.length == 0) {
                    // EMPTY
                } else {
                    var obj = jQuery.parseJSON(response.data);
                    console.log(obj);
                }
            }
        });
            }
        }
    updateModelData();
</script>
*/
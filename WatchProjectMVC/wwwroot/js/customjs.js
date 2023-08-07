window.setTimeout(function () {
    $(".alert").fadeTo(500, 0).slideUp(500, function () {
        $(this).remove();
    });
}, 4000);




    //$(() =>{
    //        getWatch();
        
    //    $('#txtSearch').on('keyup',() =>{
    //        getWatch();
    //    });
    //});
    //    function getWatch(){
    //        $.ajax({
    //            url: '@Url.Action("GetbyName","Watch")',
    //            datatype: 'html',
    //            method: 'Get',
    //            data: { searchText: $('#txtSearch').val() },
    //            success: function (res) {
    //                $('#grdWatch').html('').html(res);
    //            },
    //            error: function (err) {
    //                console.log(err);
    //            }
    //        })
    //    }


$(() => {

    $(".newperson").on('click', function () {

        $(".addperson").modal();

    });


    $(".edit-btn").on('click', function () {
        
        var ID = $(this).data('person-id');
        var date = $(this).data('date-created');
        var included = $(this).data('always-include');
        var row = $(this).closest('tr');
        var name = row.find('td:eq(1)').text();
        var cell = row.find('td:eq(2)').text();
        

        $("#person-span").text(name);
        $("#name").val(name);
        $("#cell").val(cell);
        $("#date").val(date);
        $("#person-id-hidden").val(ID);
        $("#include").prop('checked', included === "True");
            
        
        $("#editmodal").modal();

    });


    $("#addsimcha").on('click', function () {
        $("#addmodal").modal();
    })




    $(".deposit").on('click', function () {
        
        var ID = $(this).data('person-id');
        var date = $(this).data('date');
        $("#person-id").val(ID);
        $("#date").val(date);

        $("#adddeposit").modal();



    });

});
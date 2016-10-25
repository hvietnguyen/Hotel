$(document).ready(function () {
    // Display confirm div
    var confirm = $("#confirm");
    if (confirm.data("active")) { return; }
    confirm.show().data("active", true);
    setTimeout(function () {
        confirm.hide().data("active", false);
    }, 5000);

    //Enable/disable booking button
    if ($("#singleRoom").text() === "No Vacancy!") {
        $("#single").attr("disabled", true);
    } else $("#single").removeAttr("disabled");

    if ($("#doubleRoom").text() === "No Vacancy!") {
        $("#double").attr("disabled", true);
    } else $("#double").removeAttr("disabled");

    if ($("#superiorRoom").text() === "No Vacancy!") {
        $("#superior").attr("disabled", true);
    } else $("#superior").removeAttr("disabled");

    // Initialize Tooltip
    $('[data-toggle="tooltip"]').tooltip();

    // Add smooth scrolling to all links in navbar + footer link
    $(".navbar a, footer a[href='#myPage']").on('click', function (event) {

        // Make sure this.hash has a value before overriding default behavior
        if (this.hash !== "") {

            // Prevent default anchor click behavior
            event.preventDefault();

            // Store hash
            var hash = this.hash;

            // Using jQuery's animate() method to add smooth page scroll
            // The optional number (900) specifies the number of milliseconds it takes to scroll to the specified area
            $('html, body').animate({
                scrollTop: $(hash).offset().top
            }, 900, function () {

                // Add hash (#) to URL when done scrolling (default click behavior)
                window.location.hash = hash;
            });
        } // End if
    });

    $('.carousel').carousel({
        interval: 3000 //speed
    });

    $("#submit").click(function () {
        var myForm = $("#myForm");
        if (myForm.checkValidity) {
            myForm.submit();
        }
    });

    /*
    $("#bookingForm").submit(function (e) {
        var data = {
            'firstName': $("#firstName").val(),
            'lastName': $("#lastName").val(),
            'identity': $("#identity").val(),
            'checkin': $("#checkin").val(),
            'checkout': $("#checkout").val(),
            'cardType': $("#cardType").val(),
            'cardNumber': $("#cardNumber").val(),
            'nameHolder': $("#nameHolder").val()
        };
        e.preventDefault();
        $.ajax({
            url: myForm.attr('action'),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            type: myForm.attr('method'),
            data: data,//myForm.serialize(),
            encode:true
        })
        .done(function (data) {
            alert("Hi");
        });
    });
    */
})
$(document).ready(function () {
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

    // Click event: Booking room button
    // Call up modal dialog
    $("#single").click(function () {
        showForm1();
    });

    $("#double").click(function () {
        showForm1();
    });

    $("#superior").click(function () {
        showForm1();
    });

    $("#continue").click(function () {
        showForm2();
    });

    $("#goBack").click(function () {
        showForm1();
    });

    $("#submit").click(function () {
        var myForm = $("#myForm");
        if (myForm[0].checkValidity()) {
            myForm.submit();
        }else{
            showForm1();
        }
    });
})

function showForm1() {
    $("#form1").show();
    $("#form2").hide();
    $("#continue").show();
    $("#submit").hide();
    $("#goBack").hide();
}

function showForm2() {
    $("#form1").hide();
    $("#form2").show();
    $("#continue").hide();
    $("#submit").show();
    $("#goBack").show();
}
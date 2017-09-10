$(function () {
    $(".deposit").on('click', function () {
        var contributorId = $(this).data('contributor-id');
        $("#DepositModal").modal();
        $("#ContributorId").val(contributorId);
    });
});
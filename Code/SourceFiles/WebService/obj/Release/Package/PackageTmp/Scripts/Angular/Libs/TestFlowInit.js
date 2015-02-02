// init javascript stuff which happens outside of angular.
var testManager = [], testFlow; // globals
$(document).ready(function () {
    testManager = new ProjectManager();
    testManager.getProjectList();

    $("#createTestPlan").click(function () {

        $("<button/>").addClass("btn btn-primary").html("Create Test Plan").click(function () {
            testManager.createTestPlan($("#newTestPlanName").val());
            $(this).remove();
            $("#myModal").fadeOut("fast");
            $(".modal-backdrop").fadeOut("slow");
        }).prependTo("#myModal .modal-footer");

        $("#myModal").find(".modal-title").html("Create New Test Plan");
        $("#myModal").find(".modal-body").html(
            'Test Plan Name <input class="form-control" type="text" id="newTestPlanName" />');
        $("#myModal").modal('show');

    });
});
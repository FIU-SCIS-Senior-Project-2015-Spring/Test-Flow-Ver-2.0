// The ProjectManager manages the selection of projects and test plans.
function ProjectManager() {
    var instance = this;
    this.projectList = {};
    this.currentProject = {};
    this.currentProjectTestPlans = {};
    this.currentTestPlan = {};

    // sets the dropdown list with the current avaliable projects from the backend
    this.getProjectList = function () {
        instance.loader("Requesting projects...", true, true);
        $.ajax({
            url: '/api/Projects',
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                if (!data || data.length <= 0)
                    window.location = "/Collections";
                instance.registerProjects(data);
                instance.loader("Choose a project.", false, true);
            },
            error: function () {
                $("#myModal").find(".modal-title").html("Error Requesting Projects");
                $("#myModal").find(".modal-body").html(
                    'An error occured when requesting the project list from TestFlow.');
                $("#myModal").modal('show');
            }
        });
    }

    // html stuff
    this.registerProjects = function(projectData) {
        $('[aria-labelledby="projectsDDM"] li.removable').remove();
        $.each(projectData, function (key, value) {
            $("<li/>")
                .attr("id", "p" + value.Id)
                .attr("role", "presentation")
                .addClass("removable")
                .html('<a role="menuitem"><strong>' + value.Name + '</strong></a>')
                .click(function () {
                    instance.selectProject(this, value);
                })
                .prependTo('[aria-labelledby="projectsDDM"]');
        });
    }

    // sets the testplans form a given project in the drop down
    this.getTestPlanList = function (projectId) {
        instance.loader("Requesting Test Plans...", true, true);
        $.ajax({
            url: '/api/TestPlans/' + projectId,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                instance.registerTestPlans(data);
                instance.loader("Choose test plan.", false, true);
            },
            error: function () {
                $("#myModal").find(".modal-title").html("Error Requesting Project Test Plans");
                $("#myModal").find(".modal-body").html(
                    'An error occured when requesting the test plan list from TestFlow.');
                $("#myModal").modal('show');
            }
        });
    }

    // html stuff
    this.registerTestPlans = function(testPlansData) {
        $('[aria-labelledby="testplansDDM"] li.removable').remove();
        $.each(testPlansData, function (key, value) {
            $("<li/>")
                .attr("role", "presentation")
                .html('<a role="menuitem"><strong>' + value.Name + '</strong></a>')
                .addClass("removable")
                .click(function () {
                    instance.selectTestPlan(this, value);
                })
                .prependTo('[aria-labelledby="testplansDDM"]');
        });
    }

    // selection of a project
    this.selectProject = function(element, project) {
        instance.currentProject = project;
        $(element).parents().find(".active").removeClass("active");
        $(element).addClass("active");
        instance.getTestPlanList(project.Id);
    }

    // selection of a test plan
    this.selectTestPlan = function (element, testPlan) {
        instance.loader("Requesting test plan...", true, true);
        instance.currentTestPlan = testPlan;
        $(element).parents().find(".active").removeClass("active");
        $(element).addClass("active");
        instance.reloadWorkspaceDelegate(instance.currentProject.Id, instance.currentTestPlan.Id, testPlan.Name);
    }

    // create new test plan
    this.createTestPlan = function (name) {
        instance.loader("Creating test plan...", true, true);
        $.post('/api/TestPlans/create/' + instance.currentProject.Id, "=" + name, function () {
            instance.getTestPlanList(instance.currentProject.Id);
        }).fail(function () {
            $("#myModal").find(".modal-title").html("Error Creating New Test Plan");
            $("#myModal").find(".modal-body").html(
                'An error occured when creating a new test plan.');
            $("#myModal").modal('show');
        });
    }

    // controls loading graphics
    this.loader = function (text, showAnimation, fadein) {
        $(".loader .msg h1").html(text);

        if (fadein === true)
            $(".loader").fadeIn("fast");
        else
            $(".loader").fadeOut("slow");

        if (showAnimation === true)
            $(".loader .graphics").addClass("gfxgogo");
        else
            $(".loader .graphics").removeClass("gfxgogo");
    }

    // delegate which should be assigned a function from the controller to be find on test plan changes.
    this.reloadWorkspaceDelegate = function (projectId, testPlanId, testPlanName) { };
};
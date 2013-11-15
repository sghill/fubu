﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;
using FubuCsProjFile.Templating.Graph;
using FubuCsProjFile.Templating.Planning;
using FubuCsProjFile.Templating.Runtime;

namespace Fubu.Generation
{
    public class AddInput
    {
        [Description("The name of the new project")]
        public string ProjectName { get; set; }

        [Description("Project profile.  Use the --list flag to see the valid options")]
        public string Profile { get; set; }

        [Description("Specify the solution file name if there is more than one in this code tree")]
        public string SolutionFlag { get; set; }

        [Description("Used in many templates as a prefix for generted classes")]
        public string ShortNameFlag { get; set; }

        [Description("Do not generate a matching testing project.  Boo!")]
        [FlagAlias("no-tests", 'n')]
        public bool NoTestsFlag { get; set; }


        [Description("Extra options for the new application")]
        public IEnumerable<string> OptionsFlag { get; set; }

        public string DetermineShortName()
        {
            return ShortNameFlag.IsEmpty()
                ? ProjectName.Split('.').Skip(1).Join("")
                : ShortNameFlag;
        }

        public TemplateChoices ToChoices()
        {
            return new TemplateChoices
            {
                Category = "add",
                ProjectName = ProjectName,
                ProjectType = Profile,
                Options = OptionsFlag
            };
        }
    }

    public class AddCommand : FubuCommand<AddInput>
    {
        public override bool Execute(AddInput input)
        {
            string solutionFile = input.SolutionFlag ?? SolutionFinder.FindSolutionFile();

            if (solutionFile.IsEmpty()) return false;

            TemplateRequest request = BuildTemplateRequest(input, solutionFile);

            TemplatePlan plan = Templating.BuildPlan(request);
            Templating.ExecutePlan(plan);

            return true;
        }

        public static TemplateRequest BuildTemplateRequest(AddInput input, string solutionFile)
        {
            var request = new TemplateRequest
            {
                RootDirectory = Environment.CurrentDirectory,
                SolutionName = solutionFile
            };

            request.Substitutions.Set(ProjectPlan.SHORT_NAME, input.DetermineShortName());

            TemplateChoices choices = input.ToChoices();

            ProjectRequest project = Templating.Library.Graph.BuildProjectRequest(choices);

            request.AddProjectRequest(project);
            if (!input.NoTestsFlag)
            {
                request.AddMatchingTestingProject(project);
            }
            return request;
        }
    }
}
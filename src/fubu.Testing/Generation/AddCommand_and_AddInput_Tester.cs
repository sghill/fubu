﻿using System.Linq;
using Fubu.Generation;
using FubuTestingSupport;
using NUnit.Framework;

namespace fubu.Testing.Generation
{
    [TestFixture]
    public class AddCommand_and_AddInput_Tester
    {
        [Test]
        public void input_can_determine_the_short_name_if_none_is_supplied()
        {
            new AddInput {ProjectName = "MyCompany.AwesomeSauce"}
                .DetermineShortName()
                .ShouldEqual("AwesomeSauce");

            new AddInput { ProjectName = "MyCompany.Core.Consumers" }
                .DetermineShortName()
                .ShouldEqual("CoreConsumers");
        }

        [Test]
        public void input_uses_short_name_flag_if_it_exists()
        {
            new AddInput
            {
                ProjectName = "MyCompany.AwesomeSauce",
                ShortNameFlag = "Radical"
            }.DetermineShortName().ShouldEqual("Radical");
        }

        [Test]
        public void adds_a_test_project_by_default()
        {
            var input = new AddInput
            {
                Profile = "library",
                ProjectName = "Foo"
            };

            var request = AddCommand.BuildTemplateRequest(input, "MyFoo.sln");

            request.TestingProjects.Single()
                .Name.ShouldEqual("Foo.Testing");
        }

        [Test]
        public void no_tests_if_the_no_testing_flag_is_selected()
        {
            var input = new AddInput
            {
                Profile = "library",
                ProjectName = "Foo",
                NoTestsFlag = true
            };

            var request = AddCommand.BuildTemplateRequest(input, "MyFoo.sln");
            request.TestingProjects.Any()
                .ShouldBeFalse();
        }
    }
}
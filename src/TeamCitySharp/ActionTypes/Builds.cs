using System;
using System.Collections.Generic;
using System.Linq;
using EasyHttp.Http;
using TeamCitySharp.Connection;
using TeamCitySharp.DomainEntities;
using TeamCitySharp.Locators;

namespace TeamCitySharp.ActionTypes
{
    public class Builds : IBuilds
    {
        private readonly ITeamCityCaller _caller;
        private string _fields;

        internal Builds(ITeamCityCaller caller)
        {
            _caller = caller;
        }

        public IBuilds GetFields(string fields)
        {
            var newInstance = (Builds)MemberwiseClone();
            newInstance._fields = fields;
            return newInstance;
        }

        public List<Build> ByBuildLocator(IBuildLocator locator)
        {
            var buildWrapper =
              _caller.GetFormat<BuildWrapper>(ActionHelper.CreateFieldUrl("/app/rest/builds?locator={0}", _fields), locator);
            return int.Parse(buildWrapper.Count) > 0 ? buildWrapper.Build : new List<Build>();
        }

        public List<Build> ByBuildLocator(IBuildLocator locator, List<String> param)
        {
            var strParam = "";
            foreach (var tmpParam in param)
            {
                strParam += ",";
                strParam += tmpParam;
            }

            var buildWrapper =
              _caller.Get<BuildWrapper>(
                ActionHelper.CreateFieldUrl(string.Format("/app/rest/builds?locator={0}{1}", locator, strParam), _fields));

            return int.Parse(buildWrapper.Count) > 0 ? buildWrapper.Build : new List<Build>();
        }

        public Build LastBuildByAgent(string agentName)
        {
            return ByBuildLocator(BuildLocator.WithDimensions(agentName: agentName, maxResults: 1)).SingleOrDefault();
        }

        public void Add2QueueBuildByBuildConfigId(string buildConfigId)
        {
            _caller.GetFormat("/action.html?add2Queue={0}", buildConfigId);
        }

        public List<Build> SuccessfulBuildsByBuildConfigId(string buildConfigId)
        {
            return ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId),
                                                              status: BuildStatus.SUCCESS
                                    ));
        }

        public List<Build> SuccessfulBuildsByBuildConfigId(string buildConfigId, List<String> param)
        {
            return ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId),
                                                              status: BuildStatus.SUCCESS
                                    ), param);
        }


        public Build LastSuccessfulBuildByBuildConfigId(string buildConfigId)
        {
            var builds = ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId),
                                                                    status: BuildStatus.SUCCESS,
                                                                    maxResults: 1
                                          ));
            return builds != null ? builds.FirstOrDefault() : new Build();
        }

        public List<Build> FailedBuildsByBuildConfigId(string buildConfigId)
        {
            return ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId),
                                                              status: BuildStatus.FAILURE
                                    ));
        }

        public Build LastFailedBuildByBuildConfigId(string buildConfigId)
        {
            var builds = ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId),
                                                                    status: BuildStatus.FAILURE,
                                                                    maxResults: 1
                                          ));
            return builds != null ? builds.FirstOrDefault() : new Build();
        }

        public Build LastBuildByBuildConfigId(string buildConfigId)
        {
            var builds = ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId),
                                                                    maxResults: 1
                                          ));
            return builds != null ? builds.FirstOrDefault() : new Build();
        }

        public List<Build> ErrorBuildsByBuildConfigId(string buildConfigId)
        {
            return ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId),
                                                              status: BuildStatus.ERROR
                                    ));
        }

        public Build LastErrorBuildByBuildConfigId(string buildConfigId)
        {
            var builds = ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId),
                                                                    status: BuildStatus.ERROR,
                                                                    maxResults: 1
                                          ));
            return builds != null ? builds.FirstOrDefault() : new Build();
        }

        public List<Build> ByBuildConfigId(string buildConfigId, List<String> param)
        {
            return ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId)), param);
        }

        public Build ById(string id)
        {
            var build = _caller.GetFormat<Build>(ActionHelper.CreateFieldUrl("/app/rest/builds/id:{0}", _fields), id);

            return build ?? new Build();
        }

        public List<Build> ByBuildConfigId(string buildConfigId)
        {
            return ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId)
                                    ));
        }

        public List<Build> RunningByBuildConfigId(string buildConfigId)
        {
            return ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId)),
                                  new List<string> { "running:true" });
        }

        public List<Build> ByConfigIdAndTag(string buildConfigId, string tag)
        {
            return ByConfigIdAndTag(buildConfigId, new[] { tag });
        }

        public List<Build> ByConfigIdAndTag(string buildConfigId, string[] tags)
        {
            return ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId),
                                                              tags: tags
                                    ));
        }

        public List<Build> ByUserName(string userName)
        {
            return ByBuildLocator(BuildLocator.WithDimensions(
              user: UserLocator.WithUserName(userName)
                                    ));
        }

        public List<Build> AllSinceDate(DateTime date)
        {
            return ByBuildLocator(BuildLocator.WithDimensions(sinceDate: date));
        }

        public List<Build> AllRunningBuild()
        {
            var buildWrapper =
              _caller.GetFormat<BuildWrapper>(ActionHelper.CreateFieldUrl("/app/rest/builds?locator=running:true", _fields));
            return int.Parse(buildWrapper.Count) > 0 ? buildWrapper.Build : new List<Build>();
        }

        public List<Build> ByBranch(string branchName)
        {
            return ByBuildLocator(BuildLocator.WithDimensions(branch: branchName));
        }

        public List<Build> AllBuildsOfStatusSinceDate(DateTime date, BuildStatus buildStatus)
        {
            return ByBuildLocator(BuildLocator.WithDimensions(sinceDate: date, status: buildStatus));
        }

        public List<Build> NonSuccessfulBuildsForUser(string userName)
        {
            var builds = ByUserName(userName);
            if (builds == null)
                return null;

            return builds.Where(b => b.Status != "SUCCESS").ToList();
        }

        public List<Build> RetrieveEntireBuildChainFrom(string buildConfigId)
        {
            var buildWrapper =
              _caller.GetFormat<BuildWrapper>(
                ActionHelper.CreateFieldUrl(
                  "/app/rest/builds?locator=snapshotDependency:(from:(id:{0}),includeInitial:true),defaultFilter:false",
                  _fields), buildConfigId);
            return int.Parse(buildWrapper.Count) > 0 ? buildWrapper.Build : new List<Build>();
        }

        public List<Build> RetrieveEntireBuildChainTo(string buildConfigId)
        {
            var buildWrapper =
              _caller.GetFormat<BuildWrapper>(
                ActionHelper.CreateFieldUrl(
                  "/app/rest/builds?locator=snapshotDependency:(to:(id:{0}),includeInitial:true),defaultFilter:false", _fields),
                buildConfigId);
            return int.Parse(buildWrapper.Count) > 0 ? buildWrapper.Build : new List<Build>();
        }


        public Build TriggerBuild(string buildConfigId)
        {
            return TriggerBuild(buildConfigId, null, null, null, null, null);
        }

        public Build TriggerBuild(string buildConfigId, IEnumerable<Property> properties)
        {
            return TriggerBuild(buildConfigId, null, null, properties, null, null);
        }

        public Build TriggerBuild(string buildConfigId, string comment, IEnumerable<Property> properties)
        {
            return TriggerBuild(buildConfigId, comment, null, properties, null, null);
        }

        public Build TriggerBuild(string buildConfigId, string comment, string branchName, IEnumerable<Property> properties, int? agentId, bool? personal)
        {
            //<build personal="true" branchName="logicBuildBranch">
            //    <buildType id="buildConfID"/>
            //    <agent id="3"/>
            //    <comment><text>build triggering comment</text></comment>
            //    <properties>
            //        <property name="env.myEnv" value="bbb"/>
            //    </properties>
            //</build>

            var personalXml = personal == null ? "" : String.Format(" personal='{0}'", personal.Value.ToString().ToLower());
            var branchXml = string.IsNullOrEmpty(branchName) ? "" : String.Format(" branchName='{0}'", branchName);
            var buildTypeXml = string.Format("<buildType id='{0}'/>", buildConfigId);
            var agentXml = agentId == null ? "" : string.Format("<agent id='{0}'/>", agentId);
            var commentXml = string.IsNullOrEmpty(comment) ? "" : string.Format("<comment><text>{0}</text></comment>", comment);
            var propertiesXml = "";
            if (properties != null && properties.Count() > 0)
            {
                propertiesXml = "<properties>";
                foreach (var property in properties)
                {
                    propertiesXml += string.Format("<property name='{0}' value='{1}'/>", property.Name, property.Value);
                }
                propertiesXml += "</properties>";
            }

            var xmlData = string.Format("<build{0}{1}>{2}{3}{4}{5}</build>",
              personalXml,
              branchXml,
              buildTypeXml,
              agentXml,
              commentXml,
              propertiesXml);

            return _caller.Post<Build>(xmlData, HttpContentTypes.ApplicationXml, "/app/rest/buildQueue", HttpContentTypes.ApplicationJson);
        }

        public Properties GetResultingProperties(string id)
        {
            var properties = _caller.GetFormat<Properties>(ActionHelper.CreateFieldUrl("/app/rest/builds/id:{0}/resulting-properties", _fields), id);
            return properties;
        }
    }
}
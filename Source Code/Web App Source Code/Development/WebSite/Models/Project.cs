using IID.BusinessLayer.Domain;
using IID.WebSite.Helpers;

namespace IID.WebSite.Models
{
    public class Project
    {
        public Project() { }

        public Project(int projectId)
        {
            using (Entity context = new Entity())
            {
                t_project entity = context.t_project.Find(projectId);
                SetProperties(entity, true);
            }
        }

        public Project (t_project entity)
        {
            SetProperties(entity, false);
        }

        private void SetProperties(t_project entity, bool translate)
        {
            ProjectId = entity.project_id;
            OrganizationId = entity.organization_id;

            if (translate)
            {
                byte languageId = IidCulture.CurrentLanguageId;
                Name = entity.get_name_translated(languageId);
            }
            else
            {
                Name = entity.name;
            }
        }

        public int ProjectId { get; set; }
        public string Name { get; set; }
        public int OrganizationId { get; set; }
    }
}
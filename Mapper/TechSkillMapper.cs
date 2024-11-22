using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trabahuso_api.DTOs.TechSkill;
using trabahuso_api.Models;

namespace trabahuso_api.Mapper
{
    public static class TechSkillMapper
    {
        public static List<TechSkill> ToTechSkills(this List<List<ResultRow>> rows)
        {
            List<TechSkill> techSkills = [];
            foreach (List<ResultRow> row in rows)
            {

                ResultRow x = row[0];
                ResultRow y = row[1];
                ResultRow z = row[2];

                if (x.Value != null && y.Value != null && z.Value != null)
                {
                    techSkills.Add(
                        new TechSkill()
                        {
                            TechStackId = x.Value,
                            JobDataId = y.Value,
                            TechType = z.Value,
                        }
                    );
                }

            }

            return techSkills;
        }

        public static TechSkill? ToTechSkill(this List<ResultRow> row)
        {

            ResultRow x = row[0];
            ResultRow y = row[1];
            ResultRow z = row[2];

            if (x.Value != null && y.Value != null && z.Value != null)
            {
                return new TechSkill()
                {
                    TechStackId = x.Value,
                    JobDataId = y.Value,
                    TechType = z.Value,
                };
            }

            return null;
        }

        public static TechSkillDto ToTechSkillDto(this TechSkill techSkill)
        {
            return new TechSkillDto(techSkill.TechStackId, techSkill.TechType);
        }
    }
}
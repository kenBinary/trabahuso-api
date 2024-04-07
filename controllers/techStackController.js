require("dotenv").config();
const asyncHandler = require("express-async-handler");
const db = require("../helpers/dbConnection");
const normalizedTechData = require(`../${process.env.NORMALIZED_TECH_KEYWORDS}`);
const techCategories = require(`../${process.env.TECH_CATEGORIES}`);

exports.getTechFrequency = asyncHandler(async (req, res) => {
  try {
    const { category } = req.query;
    const technologiesQuery = db.prepare("SELECT tech_type FROM tech_skill");
    const technologies = technologiesQuery.all();

    let techPopularity = {};
    technologies.forEach((element) => {
      const tech = element["tech_type"];
      if (tech in techPopularity) {
        techPopularity[tech] = techPopularity[tech] + 1;
      } else {
        techPopularity[tech] = 1;
      }
    });

    // normalize data
    let normTechPop = {};
    for (const tech in techPopularity) {
      if (tech in normalizedTechData) {
        if (tech in normTechPop) {
          normTechPop[tech] = normTechPop[tech] + techPopularity[tech];
        } else {
          normTechPop[tech] = techPopularity[tech];
        }
      } else {
        for (const normalizedTech in normalizedTechData) {
          const rawData = normalizedTechData[normalizedTech];
          if (rawData.includes(tech) && normalizedTech in normTechPop) {
            normTechPop[normalizedTech] =
              normTechPop[normalizedTech] + techPopularity[tech];
          } else if (
            rawData.includes(tech) &&
            !(normalizedTech in normTechPop)
          ) {
            normTechPop[normalizedTech] = techPopularity[tech];
          }
        }
      }
    }

    // filter
    switch (category) {
      case "cloud_platforms": {
        const techList = techCategories[category];
        for (const tech in normTechPop) {
          if (!techList.includes(tech)) {
            delete normTechPop[tech];
          }
        }
        break;
      }
      case "databases": {
        const techList = techCategories[category];
        for (const tech in normTechPop) {
          if (!techList.includes(tech)) {
            delete normTechPop[tech];
          }
        }
        break;
      }
      case "cloud_platformss": {
        const techList = techCategories[category];
        for (const tech in normTechPop) {
          if (!techList.includes(tech)) {
            delete normTechPop[tech];
          }
        }
        break;
      }
      case "programming_languages": {
        const techList = techCategories[category];
        for (const tech in normTechPop) {
          if (!techList.includes(tech)) {
            delete normTechPop[tech];
          }
        }
        break;
      }
      case "frameworks_and_libraries": {
        const techList = techCategories[category];
        for (const tech in normTechPop) {
          if (!techList.includes(tech)) {
            delete normTechPop[tech];
          }
        }
        break;
      }
      case "tools": {
        const techList = techCategories[category];
        for (const tech in normTechPop) {
          if (!techList.includes(tech)) {
            delete normTechPop[tech];
          }
        }
        break;
      }
      default:
        break;
    }

    // sort
    let sortedEntries = Object.entries(normTechPop).sort((a, b) => {
      return b[1] - a[1];
    });
    const sortedTechPop = {};
    sortedEntries.forEach((element) => {
      sortedTechPop[element[0]] = element[1];
    });

    res.status(200).json(sortedTechPop);
  } catch (error) {
    console.error(error);
    res.status(400).json({
      message: "error",
    });
  }
});

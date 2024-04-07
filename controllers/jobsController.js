require("dotenv").config();
const asyncHandler = require("express-async-handler");
const db = require("../helpers/dbConnection");
const stat = require("../utils/statUtils");
const normalizedTechData = require(`../${process.env.NORMALIZED_TECH_KEYWORDS}`);
const techCategories = require(`../${process.env.TECH_CATEGORIES}`);

exports.getJobs = asyncHandler(async (req, res) => {
  try {
    const months = {
      jan: 1,
      feb: 2,
      mar: 3,
      apr: 4,
      may: 5,
      june: 6,
      july: 7,
      aug: 8,
      sept: 9,
      oct: 10,
      nov: 11,
      dec: 12,
    };
    const { month, year, province } = req.query;

    const statement = db.prepare(
      "SELECT job_title, location, salary, job_level, date_scraped FROM job_data"
    );
    let jobs = statement.all();

    if (year) {
      jobs = jobs.filter((job) => {
        const jobYear = Number(job["date_scraped"].split("-")[0]);
        console.log(jobYear);
        return Number(year) === jobYear;
      });
    }

    if (month) {
      jobs = jobs.filter((job) => {
        const jobMonth = Number(job["date_scraped"].split("-")[1]);
        return months[month] === jobMonth;
      });
    }

    if (province) {
      jobs = jobs.filter((job) => {
        const jobLocation = job["location"].split(",")[0];
        return province.toUpperCase() === jobLocation.toUpperCase();
      });
    }

    res.status(200).send(jobs);
  } catch (error) {
    res.status(400).send("error occured while retrieving jobs");
  }
});

exports.getJobLocations = asyncHandler(async (req, res) => {
  try {
    const statement = db.prepare(
      "SELECT location, salary, date_scraped FROM job_data"
    );
    let jobs = statement.all();

    let locationData = {};

    let addedLocations = [];
    jobs.forEach((job) => {
      const province = job["location"].split(",")[0];
      const salary = job["salary"];

      if (addedLocations.includes(province)) {
        locationData[province]["jobCount"] += 1;
        if (salary) {
          locationData[province]["medianSalary"].push(salary);
        }
      } else {
        locationData[province] = {
          jobCount: 1,
          medianSalary: salary ? [salary] : [],
        };
      }
      addedLocations.push(province);
    });

    for (const province in locationData) {
      const salaryList = locationData[province]["medianSalary"].sort();
      let mid = Math.floor(salaryList.length / 2);
      if (salaryList.length === 2) {
        locationData[province]["medianSalary"] =
          (salaryList[0] + salaryList[1]) / 2;
      } else {
        locationData[province]["medianSalary"] = salaryList[mid];
      }
    }

    res.status(200).json(locationData);
  } catch (error) {
    res.status(400).send("error");
  }
});

exports.getJobByProvince = asyncHandler(async (req, res) => {
  try {
    // TODO: filter by date scraped
    let { province } = req.params;

    const wordSplit = province.split("-");
    if (wordSplit.length > 0) {
      province = wordSplit.join(" ");
    }

    let jobDetail = {
      location: "",
      jobCount: 0,
      medianSalary: 0,
    };

    const jobCountQuery = db.prepare(
      "select count(location) as job_count from job_data where SUBSTR(location,0, instr(location, ',')) = ?;"
    );
    let job = jobCountQuery.get(province.toUpperCase());
    jobDetail.jobCount = job.job_count;

    const jobSalariesQuery = db.prepare(
      "select salary from job_data where SUBSTR(location,0, instr(location, ',')) = ?;"
    );
    jobSalariesQuery.raw();
    const salaryList = jobSalariesQuery
      .all(province.toUpperCase())
      .flat()
      .sort();
    jobSalariesQuery.raw(false);

    if (salaryList.length === 2) {
      jobDetail.medianSalary = (salaryList[0] + salaryList[1]) / 2;
    } else {
      jobDetail.medianSalary =
        salaryList[Math.floor(salaryList.length - 1 / 2)];
    }

    const jobLocationQuery = db.prepare(
      "select location from job_data where SUBSTR(location,0, instr(location, ',')) = ?;"
    );
    const jobLocation = jobLocationQuery.get(province.toUpperCase());
    jobDetail.location = jobLocation["location"];

    res.status(200).json(jobDetail);
  } catch (error) {
    res.status(400).send("error");
  }
});

exports.getSalary = asyncHandler(async (req, res) => {
  try {
    let salaryDetails = {
      undisclosed: null,
      disclosed: null,
    };

    const salaryListQuery = db.prepare(
      "SELECT salary FROM job_data where salary is not null"
    );
    salaryListQuery.raw();
    const salaryList = salaryListQuery
      .all()
      .flat()
      .sort((a, b) => a - b);

    const nullSalaryListQuery = db.prepare(
      "SELECT salary FROM job_data where salary is null"
    );
    nullSalaryListQuery.raw();
    const nullSalaryList = nullSalaryListQuery.all().flat();

    let frequencyDistribution = stat.getFrequencyDistribution(salaryList);

    for (const salaryRange in frequencyDistribution) {
      const [min, max] = salaryRange.split("-");
      let rangeCount = 0;
      salaryList.forEach((salary) => {
        if (salary >= Number(min) && salary <= max) {
          rangeCount += 1;
        }
      });
      frequencyDistribution[salaryRange] = rangeCount;
    }

    salaryDetails.undisclosed = nullSalaryList.length;
    salaryDetails.disclosed = frequencyDistribution;

    res.status(200).json(salaryDetails);
  } catch (error) {
    console.error(error);
    res.status(400).send("error");
  }
});

exports.getTechPopularity = asyncHandler(async (req, res) => {
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

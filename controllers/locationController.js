require("dotenv").config();
const asyncHandler = require("express-async-handler");
const db = require("../helpers/dbConnection");

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

    const jobLocationQuery = db.prepare(
      "select location from job_data where SUBSTR(location,0, instr(location, ',')) = ?;"
    );
    const jobLocation = jobLocationQuery.get(province.toUpperCase());
    if (!jobLocation) {
      throw new Error(`not found`, { cause: `(${province}) not found` });
    }
    jobDetail.location = jobLocation["location"];

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

    res.status(200).json(jobDetail);
  } catch (error) {
    console.error(error.message);
    if (error.message === "not found") {
      res.status(404).json({
        message: error.cause,
      });
    } else {
      res.status(500).send("error");
    }
  }
});

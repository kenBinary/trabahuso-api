const express = require("express");
const router = express.Router();
const salaryController = require("../controllers/salaryController");

// TODO:query string for filtering jobs by month
router.get("/", salaryController.getSalary);

module.exports = router;

const express = require("express");
const router = express.Router();
const locationController = require("../controllers/locationController");

router.get("/", locationController.getJobLocations);
router.get("/:province", locationController.getJobByProvince);

module.exports = router;

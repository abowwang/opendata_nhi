SELECT * FROM `nhi_gauzemask` 
LEFT JOIN `nhi_gauzemask_location` ON `nhi_gauzemask`.`organization_id` = `nhi_gauzemask_location`.`organization_id` 
WHERE `nhi_gauzemask_location`.`lat` IS NULL OR `nhi_gauzemask_location`.`lng` IS NULL
LIMIT 500;
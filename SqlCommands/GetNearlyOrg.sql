SELECT 
  `nhi_gauzemask`.`organization_id`,
  `nhi_gauzemask`.`organization_name`,
  `nhi_gauzemask`.`organization_addr`,
  `nhi_gauzemask`.`organization_tel`,
  `nhi_gauzemask`.`human_count`,
  `nhi_gauzemask`.`child_count`,
  `nhi_gauzemask`.`updated_at`,
  `nhi_gauzemask_location`.`lat` AS "lat",
  `nhi_gauzemask_location`.`lng` AS "lng",
   111.111 * DEGREES(ACOS(LEAST(1.0, COS(RADIANS({0}))
         * COS(RADIANS(`nhi_gauzemask_location`.`lat`))
         * COS(RADIANS({1} - `nhi_gauzemask_location`.`lng`))
         + SIN(RADIANS({2}))
         * SIN(RADIANS(`nhi_gauzemask_location`.`lat`))))) AS distance_in_km
FROM `nhi_gauzemask` 
INNER JOIN `nhi_gauzemask_location` on `nhi_gauzemask`.`organization_id` = `nhi_gauzemask_location`.`organization_id`
WHERE
    MBRContains (
        LineString (
            Point ( {3} + 50 / ( 111.1 / COS( RADIANS( 0 ) ) ),{4} + 50 / 111.1  ),
            Point ( {5} - 50 / ( 111.1 / COS( RADIANS( 0 ) ) ), {6} - 50 / 111.1  )  
            ),
        `nhi_gauzemask_location`.`coord`
    )
ORDER BY distance_in_km 
LIMIT 20;

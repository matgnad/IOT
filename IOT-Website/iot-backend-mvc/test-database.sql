-- ================================================
-- MQTT System Database Health Check
-- ================================================

USE iot_db;

-- [1] Check table structure
SELECT 'Table Structure:' as '';
DESCRIBE sensors;

-- [2] Check total records
SELECT 'Total Records:' as '';
SELECT COUNT(*) as total_records FROM sensors;

-- [3] Check latest 5 records
SELECT 'Latest 5 Records:' as '';
SELECT 
    id,
    temperature,
    humidity,
    measured_at,
    TIMESTAMPDIFF(SECOND, measured_at, NOW()) as seconds_ago
FROM sensors 
ORDER BY id DESC 
LIMIT 5;

-- [4] Check if data is fresh (should be < 10 seconds)
SELECT 'Data Freshness Check:' as '';
SELECT 
    MAX(measured_at) as last_update,
    TIMESTAMPDIFF(SECOND, MAX(measured_at), NOW()) as seconds_since_last,
    CASE 
        WHEN TIMESTAMPDIFF(SECOND, MAX(measured_at), NOW()) < 10 THEN '✅ HEALTHY - Data is fresh'
        WHEN TIMESTAMPDIFF(SECOND, MAX(measured_at), NOW()) < 60 THEN '⚠️ WARNING - Data is stale'
        ELSE '❌ ERROR - No recent data'
    END as status
FROM sensors;

-- [5] Check recent activity (last 1 minute)
SELECT 'Recent Activity (Last Minute):' as '';
SELECT 
    COUNT(*) as records_last_minute,
    MIN(temperature) as min_temp,
    MAX(temperature) as max_temp,
    AVG(temperature) as avg_temp,
    MIN(humidity) as min_humid,
    MAX(humidity) as max_humid,
    AVG(humidity) as avg_humid
FROM sensors
WHERE measured_at > DATE_SUB(NOW(), INTERVAL 1 MINUTE);

-- [6] Test manual insert
SELECT 'Testing Manual Insert:' as '';
INSERT INTO sensors (temperature, humidity) VALUES (99.9, 88.8);
SELECT 'Manual insert successful!' as result;

-- Clean up test record
DELETE FROM sensors WHERE temperature = 99.9 AND humidity = 88.8 ORDER BY id DESC LIMIT 1;
SELECT 'Test record cleaned up' as result;


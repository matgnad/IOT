import SensorData from "../models/SensorData.js";
import { Op } from "sequelize";

class SensorsController {
    // ================================
    // 1) Lấy 20 giá trị mới nhất
    // ================================
    async latest(req, res) {
        try {
            const rows = await SensorData.findAll({
                order: [["id", "DESC"]],
                limit: 20
            });

            return res.json({
                success: true,
                data: rows
            });
        } catch (err) {
            console.error("Error in latest:", err);
            return res.status(500).json({ success: false, message: "Server error" });
        }
    }

    // ================================
    // 2) Lấy dữ liệu có phân trang + tìm kiếm
    // ================================
    async list(req, res) {
        try {
            const page = Number(req.query.page || 1);
            const limit = Number(req.query.limit || 10);
            const offset = (page - 1) * limit;

            const search = req.query.search || "";
            const field = req.query.field || "temperature";
            const order = req.query.order || "DESC";

            let where = {};

            if (search.trim() !== "") {
                if (["temperature", "humidity"].includes(field)) {
                    where[field] = {
                        [Op.like]: `%${search}%`
                    };
                }
            }

            const { count, rows } = await SensorData.findAndCountAll({
                where,
                limit,
                offset,
                order: [["id", order]]
            });

            return res.json({
                success: true,
                page,
                limit,
                total: count,
                data: rows
            });

        } catch (err) {
            console.error("Error in list:", err);
            return res.status(500).json({ success: false, message: "Server error" });
        }
    }

    // ================================
    // 3) Tính thống kê (dashboard)
    // ================================
    async stats(req, res) {
        try {
            const rows = await SensorData.findAll({
                order: [["id", "DESC"]],
                limit: 50
            });

            const temps = rows.map(r => Number(r.temperature));
            const humids = rows.map(r => Number(r.humidity));

            return res.json({
                success: true,
                temperature: temps,
                humidity: humids,
                labels: rows.map(r => r.measured_at)
            });

        } catch (err) {
            console.error("Error in stats:", err);
            return res.status(500).json({ success: false, message: "Server error" });
        }
    }
}

export default new SensorsController();

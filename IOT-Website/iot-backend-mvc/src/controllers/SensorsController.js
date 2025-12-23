import SensorData from '../models/SensorData.js';
import { Op } from 'sequelize';

const SensorsController = {

  // GET /api/sensors/latest
  async latest(req, res) {
    try {
      const latest = await SensorData.findOne({
        order: [['measured_at', 'DESC']]
      });

      if (!latest) {
        return res.json({
          success: false,
          message: 'No sensor data available',
          data: null
        });
      }

      return res.json({
        success: true,
        data: {
          id: latest.id,
          temperature: latest.temperature,
          humidity: latest.humidity,
          measured_at: latest.measured_at
        }
      });
    } catch (err) {
      console.error('[SensorsController.latest] Error:', err);
      return res.status(500).json({ 
        success: false,
        message: 'Server error',
        error: err.message
      });
    }
  },

  // GET /api/sensors
  async list(req, res) {
    try {
      const {
        page = 1,
        limit = 10,
        order = 'DESC'
      } = req.query;

      const offset = (page - 1) * limit;

      const { rows, count } = await SensorData.findAndCountAll({
        order: [['measured_at', order]],
        limit: Number(limit),
        offset: Number(offset),
        raw: true  // Return plain objects instead of Sequelize instances
      });

      return res.json({
        success: true,
        data: rows,
        pagination: {
          page: Number(page),
          limit: Number(limit),
          total: count,
          totalPages: Math.ceil(count / limit)
        }
      });
    } catch (err) {
      console.error('[SensorsController.list] Error:', err);
      return res.status(500).json({ 
        success: false,
        message: 'Server error',
        error: err.message
      });
    }
  },

  // GET /api/sensors/today
  async today(req, res) {
    try {
      const today = new Date();
      today.setHours(0, 0, 0, 0);

      const data = await SensorData.findAll({
        where: {
          measured_at: {
            [Op.gte]: today
          }
        },
        order: [['measured_at', 'ASC']],
        raw: true  // Return plain objects instead of Sequelize instances
      });

      return res.json({
        success: true,
        data: data
      });
    } catch (err) {
      console.error('[SensorsController.today] Error:', err);
      return res.status(500).json({ 
        success: false,
        message: 'Server error',
        error: err.message
      });
    }
  }
};

export default SensorsController;

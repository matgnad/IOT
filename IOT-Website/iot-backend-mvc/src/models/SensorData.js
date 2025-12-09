import { DataTypes, Model } from "sequelize";
import { sequelize } from "../config/database.js";

class SensorData extends Model {}

SensorData.init(
  {
    id: {
      type: DataTypes.BIGINT,
      autoIncrement: true,
      primaryKey: true,
    },

    temperature: {
      type: DataTypes.FLOAT,
      allowNull: false,
    },

    humidity: {
      type: DataTypes.FLOAT,
      allowNull: false,
    },

    measured_at: {
      type: DataTypes.DATE,
      allowNull: false,
      defaultValue: DataTypes.NOW,
    },
  },
  {
    sequelize,
    modelName: "SensorData",
    tableName: "sensors", // bảng bạn tạo
    timestamps: false,
  }
);

export default SensorData;

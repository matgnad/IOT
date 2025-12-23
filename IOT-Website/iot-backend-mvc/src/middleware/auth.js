export function verifyToken(req, res, next) {
  const authHeader = req.headers.authorization;

  if (!authHeader) {
    return res.status(401).json({
      success: false,
      message: "Missing Authorization header"
    });
  }

  // Defensive: handle malformed Authorization header
  const parts = authHeader.split(" ");
  if (parts.length !== 2 || parts[0] !== "Bearer") {
    return res.status(401).json({
      success: false,
      message: "Malformed Authorization header. Expected format: Bearer <token>"
    });
  }

  const token = parts[1];

  if (!token || token !== process.env.API_TOKEN) {
    return res.status(403).json({
      success: false,
      message: "Invalid token"
    });
  }

  next();
}

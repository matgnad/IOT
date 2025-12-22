export function verifyToken(req, res, next) {
  const authHeader = req.headers.authorization;

  if (!authHeader) {
    return res.status(401).json({
      success: false,
      message: "Missing Authorization header"
    });
  }

  const token = authHeader.split(" ")[1]; // Bearer xxx

  if (token !== process.env.API_TOKEN) {
    return res.status(403).json({
      success: false,
      message: "Invalid token"
    });
  }

  next();
}

# Nautilus Email Service

A standalone Express service for sending emails using Resend and React Email templates.

## Setup

1. Install dependencies:
```bash
npm install
```

2. Configure environment variables in `src/.env`:
```
RESEND_API_KEY=your_resend_api_key
PORT=3001
```

## Running the Service

Development mode:
```bash
npm run dev
```

Production build:
```bash
npm run build
npm start
```

## API Endpoints

### Health Check
```
GET /health
```

### Send Activation Email
```
POST /api/send/activate
Content-Type: application/json

{
  "email": "user@example.com",
  "name": "John Doe",
  "activationUrl": "https://nautilus.app/activate?token=..."
}
```

### Send Password Reset Email
```
POST /api/send/reset-password
Content-Type: application/json

{
  "email": "user@example.com",
  "name": "John Doe",
  "resetUrl": "https://nautilus.app/reset-password?token=..."
}
```

## Templates

Email templates are located in `src/templates/`:
- `ActivateAccount.tsx` - Account activation email
- `ResetPassword.tsx` - Password reset email

## Integration with .NET API

From your .NET backend, make HTTP requests to this service:

```csharp
var client = new HttpClient();
var request = new
{
    email = "user@example.com",
    name = "John Doe",
    activationUrl = "https://nautilus.app/activate?token=xyz"
};

var response = await client.PostAsJsonAsync(
    "http://localhost:3001/api/send/activate",
    request
);
```

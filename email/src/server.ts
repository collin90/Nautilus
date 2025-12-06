import express, { Request, Response } from 'express';
import { Resend } from 'resend';
import { render } from '@react-email/render';
import ActivateAccount from './templates/ActivateAccount';
import ResetPassword from './templates/ResetPassword';
import dotenv from 'dotenv';

dotenv.config();

const app = express();
const port = process.env.PORT || 3001;
const isDevelopment = process.env.NODE_ENV !== 'production';

// Initialize Resend
const resend = new Resend(process.env.RESEND_API_KEY);

// Helper function to convert email to test domain in development
function getEmailRecipient(email: string): string {
    if (!isDevelopment) {
        return email;
    }

    if (email == "nautilus-devs@outlook.com") {
        return email;
    }

    // Replace domain with nautilus.testing for development
    const [localPart] = email.split('@');
    return `delivered+${localPart}@resend.dev`;
}

app.use(express.json());

// CORS middleware (adjust origin for production)
app.use((req, res, next) => {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Allow-Headers', 'Content-Type');
    res.header('Access-Control-Allow-Methods', 'POST, OPTIONS');
    if (req.method === 'OPTIONS') {
        return res.sendStatus(200);
    }
    next();
});

// Health check endpoint
app.get('/health', (_req: Request, res: Response) => {
    res.json({ status: 'ok', service: 'nautilus-email' });
});

// Send activation email
app.post('/api/send/activate', async (req: Request, res: Response) => {
    try {
        const { email, name, activationUrl } = req.body;

        if (!email || !name || !activationUrl) {
            return res.status(400).json({
                error: 'Missing required fields: email, name, activationUrl'
            });
        }

        const emailHtml = await render(
            ActivateAccount({ activationUrl, name })
        );

        const recipientEmail = getEmailRecipient(email);
        console.log(`Sending activation email to: ${recipientEmail}${isDevelopment ? ' (test mode)' : ''}`);

        const { data, error } = await resend.emails.send({
            from: 'Nautilus <noreply-nautilus@resend.dev>',
            to: [recipientEmail],
            subject: 'Activate Your Nautilus Account',
            html: emailHtml,
        });

        if (error) {
            console.error('Resend error:', error);
            return res.status(400).json({ error: error.message });
        }

        res.status(200).json({ success: true, messageId: data?.id });
    } catch (error: any) {
        console.error('Error sending activation email:', error);
        res.status(500).json({ error: error.message || 'Failed to send email' });
    }
});

// Send password reset email
app.post('/api/send/reset-password', async (req: Request, res: Response) => {
    try {
        const { email, name, resetUrl } = req.body;

        if (!email || !name || !resetUrl) {
            return res.status(400).json({
                error: 'Missing required fields: email, name, resetUrl'
            });
        }

        const emailHtml = await render(
            ResetPassword({ resetUrl, name })
        );

        const recipientEmail = getEmailRecipient(email);
        console.log(`Sending password reset email to: ${recipientEmail}${isDevelopment ? ' (test mode)' : ''}`);

        const { data, error } = await resend.emails.send({
            from: 'Nautilus <noreply-nautilus@resend.dev>',
            to: [recipientEmail],
            subject: 'Reset Your Nautilus Password',
            html: emailHtml,
        });

        if (error) {
            console.error('Resend error:', error);
            return res.status(400).json({ error: error.message });
        }

        res.status(200).json({ success: true, messageId: data?.id });
    } catch (error: any) {
        console.error('Error sending reset password email:', error);
        res.status(500).json({ error: error.message || 'Failed to send email' });
    }
});

app.listen(port, () => {
    console.log(`📧 Email service running on port ${port}`);
    console.log(`Health check: http://localhost:${port}/health`);
});
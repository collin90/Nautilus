import { Html, Text, Button, Container, Heading, Section } from "@react-email/components";

interface Props {
    resetUrl: string;
    name: string;
}

export default function ResetPassword({ resetUrl, name }: Props) {
    return (
        <Html>
            <Container style={{ padding: "24px", fontFamily: "sans-serif" }}>
                <Heading style={{ fontSize: "20px", marginBottom: "16px" }}>
                    Reset Your Password
                </Heading>

                <Text>Hello {name},</Text>
                <Text>
                    We received a request to reset your password. Click the button below to continue:
                </Text>

                <Section style={{ margin: "24px 0" }}>
                    <Button
                        href={resetUrl}
                        style={{
                            backgroundColor: "#b91c1c",
                            padding: "12px 20px",
                            color: "#fff",
                            borderRadius: "6px",
                            textDecoration: "none"
                        }}
                    >
                        Reset Password
                    </Button>
                </Section>

                <Text>
                    If the button doesn't work, copy and paste this URL:
                    <br />
                    {resetUrl}
                </Text>

                <Text style={{ color: "#888", marginTop: "24px" }}>
                    — The Nautilus Team
                </Text>
            </Container>
        </Html>
    );
}

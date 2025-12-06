import { Html, Button, Text, Section, Container, Heading } from "@react-email/components";

interface Props {
    activationUrl: string;
    name: string;
}

export default function ActivateAccount({ activationUrl, name }: Props) {
    return (
        <Html>
            <Container style={{ padding: "24px", fontFamily: "sans-serif" }}>
                <Heading style={{ fontSize: "20px", marginBottom: "16px" }}>
                    Activate Your Nautilus Account
                </Heading>

                <Text>Hello {name},</Text>
                <Text>Thanks for signing up! Click the button below to activate your account.</Text>

                <Section style={{ margin: "24px 0" }}>
                    <Button
                        href={activationUrl}
                        style={{
                            backgroundColor: "#1d4ed8",
                            padding: "12px 20px",
                            color: "#fff",
                            borderRadius: "6px",
                            textDecoration: "none"
                        }}
                    >
                        Activate Account
                    </Button>
                </Section>

                <Text>
                    If the button doesn't work, copy and paste this link:
                    <br />
                    {activationUrl}
                </Text>

                <Text style={{ color: "#888", marginTop: "24px" }}>
                    — The Nautilus Team
                </Text>
            </Container>
        </Html>
    );
}

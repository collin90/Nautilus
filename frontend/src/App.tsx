import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Page } from "@/components/layout/page";

export default function App() {
  return (
    <Page>
      <Card className="w-[350px]">
        <CardHeader>
          <CardTitle className="text-center">Hello World</CardTitle>
        </CardHeader>
        <CardContent className="text-center text-muted-foreground">
          Welcome to your beautiful new Shadcn + Vite + React app ✨
        </CardContent>
      </Card>
    </Page>
  );
}

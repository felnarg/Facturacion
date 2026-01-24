import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import { AuthProvider } from "@/components/AuthProvider";
import { AppShell } from "@/components/AppShell";
import { DevModeProvider } from "@/context/DevModeContext";
import { DevModeToggle } from "@/components/DevModeToggle";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Zupply - Panel Operativo",
  description: "Interfaz para microservicios de facturación",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="es">
      <body
        className={`${geistSans.variable} ${geistMono.variable} antialiased`}
      >
        <DevModeProvider>
          <DevModeToggle />
          <AuthProvider>
            <AppShell>{children}</AppShell>
          </AuthProvider>
        </DevModeProvider>
      </body>
    </html>
  );
}

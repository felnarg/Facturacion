namespace Notificaciones.Application.Templates;

/// <summary>
/// Genera el HTML del email enviado al cliente cuando su factura
/// ha sido transmitida a la DIAN (aceptada o rechazada).
/// </summary>
public static class FacturaClienteHtmlTemplate
{
    public static string Generar(
        string nombreCliente,
        string numeroDocumento,
        string cufe,
        DateTime fechaEmision,
        decimal subtotal,
        decimal totalImpuestos,
        decimal total,
        bool aceptado,
        string respuestaDian,
        string emisorNombre)
    {
        var estadoColor  = aceptado ? "#10b981" : "#ef4444";
        var estadoTexto  = aceptado ? "✅ ACEPTADA POR LA DIAN" : "❌ RECHAZADA POR LA DIAN";
        var estadoBg     = aceptado ? "#d1fae5" : "#fee2e2";
        var qrUrl        = $"https://catalogo-vpfe-hab.dian.gov.co/document/searchqr?documentkey={cufe}";

        return $"""
        <!DOCTYPE html>
        <html lang="es">
        <head>
          <meta charset="UTF-8">
          <meta name="viewport" content="width=device-width, initial-scale=1.0">
          <title>Factura Electrónica {numeroDocumento}</title>
        </head>
        <body style="margin:0;padding:0;background-color:#f3f4f6;font-family:'Segoe UI',Arial,sans-serif;">
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#f3f4f6;padding:32px 0;">
            <tr>
              <td align="center">
                <table width="600" cellpadding="0" cellspacing="0" style="background:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,0.08);">

                  <!-- Header -->
                  <tr>
                    <td style="background:linear-gradient(135deg,#1e3a5f,#2563eb);padding:36px 40px;text-align:center;">
                      <h1 style="color:#ffffff;margin:0;font-size:22px;font-weight:700;letter-spacing:-0.5px;">
                        🧾 Factura Electrónica
                      </h1>
                      <p style="color:#93c5fd;margin:8px 0 0;font-size:14px;">{emisorNombre}</p>
                    </td>
                  </tr>

                  <!-- Estado DIAN -->
                  <tr>
                    <td style="padding:24px 40px 0;">
                      <div style="background:{estadoBg};border-radius:8px;padding:16px 20px;text-align:center;">
                        <span style="color:{estadoColor};font-size:16px;font-weight:700;">{estadoTexto}</span>
                        <p style="color:#6b7280;font-size:13px;margin:6px 0 0;">{respuestaDian}</p>
                      </div>
                    </td>
                  </tr>

                  <!-- Saludo -->
                  <tr>
                    <td style="padding:24px 40px 16px;">
                      <p style="color:#111827;font-size:16px;margin:0;">
                        Estimado/a <strong>{nombreCliente}</strong>,
                      </p>
                      <p style="color:#6b7280;font-size:14px;margin:8px 0 0;line-height:1.6;">
                        Le informamos que su factura electrónica ha sido procesada. A continuación encontrará
                        los detalles del documento emitido:
                      </p>
                    </td>
                  </tr>

                  <!-- Detalles del documento -->
                  <tr>
                    <td style="padding:0 40px;">
                      <table width="100%" cellpadding="12" cellspacing="0" style="border-collapse:collapse;border-radius:8px;overflow:hidden;border:1px solid #e5e7eb;">
                        <tr style="background:#f9fafb;">
                          <td style="font-size:13px;color:#6b7280;font-weight:600;border-bottom:1px solid #e5e7eb;">Número de Factura</td>
                          <td style="font-size:14px;color:#111827;font-weight:700;border-bottom:1px solid #e5e7eb;">{numeroDocumento}</td>
                        </tr>
                        <tr>
                          <td style="font-size:13px;color:#6b7280;font-weight:600;border-bottom:1px solid #e5e7eb;">Fecha de Emisión</td>
                          <td style="font-size:14px;color:#111827;border-bottom:1px solid #e5e7eb;">{fechaEmision:dd/MM/yyyy HH:mm}</td>
                        </tr>
                        <tr style="background:#f9fafb;">
                          <td style="font-size:13px;color:#6b7280;font-weight:600;border-bottom:1px solid #e5e7eb;">Subtotal</td>
                          <td style="font-size:14px;color:#111827;border-bottom:1px solid #e5e7eb;">{subtotal:C2}</td>
                        </tr>
                        <tr>
                          <td style="font-size:13px;color:#6b7280;font-weight:600;border-bottom:1px solid #e5e7eb;">IVA</td>
                          <td style="font-size:14px;color:#111827;border-bottom:1px solid #e5e7eb;">{totalImpuestos:C2}</td>
                        </tr>
                        <tr style="background:#eff6ff;">
                          <td style="font-size:14px;color:#1e3a5f;font-weight:700;">TOTAL A PAGAR</td>
                          <td style="font-size:18px;color:#1e3a5f;font-weight:800;">{total:C2} COP</td>
                        </tr>
                      </table>
                    </td>
                  </tr>

                  <!-- CUFE -->
                  <tr>
                    <td style="padding:20px 40px 0;">
                      <div style="background:#f8fafc;border:1px solid #e2e8f0;border-radius:8px;padding:16px;">
                        <p style="color:#64748b;font-size:12px;font-weight:600;margin:0 0 6px;text-transform:uppercase;letter-spacing:0.5px;">
                          Código Único de Factura Electrónica (CUFE)
                        </p>
                        <p style="font-family:'Courier New',monospace;font-size:11px;color:#334155;word-break:break-all;margin:0;line-height:1.5;">
                          {cufe}
                        </p>
                      </div>
                    </td>
                  </tr>

                  <!-- Verificar en DIAN -->
                  <tr>
                    <td style="padding:20px 40px 0;text-align:center;">
                      <a href="{qrUrl}"
                         style="display:inline-block;background:linear-gradient(135deg,#1e3a5f,#2563eb);color:#ffffff;text-decoration:none;padding:12px 28px;border-radius:8px;font-size:14px;font-weight:600;">
                        🔍 Verificar en Portal DIAN
                      </a>
                    </td>
                  </tr>

                  <!-- Footer -->
                  <tr>
                    <td style="padding:32px 40px;text-align:center;border-top:1px solid #e5e7eb;margin-top:24px;">
                      <p style="color:#9ca3af;font-size:12px;margin:0;line-height:1.6;">
                        Este mensaje fue generado automáticamente por el Sistema de Facturación Electrónica.<br>
                        Por favor no responda a este correo.<br>
                        <strong>{emisorNombre}</strong>
                      </p>
                    </td>
                  </tr>

                </table>
              </td>
            </tr>
          </table>
        </body>
        </html>
        """;
    }
}

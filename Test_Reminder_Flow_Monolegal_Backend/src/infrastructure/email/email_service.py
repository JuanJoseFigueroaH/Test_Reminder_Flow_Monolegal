import aiosmtplib
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart

from src.domain.entities.invoice import Invoice
from src.domain.ports.email_service_port import EmailServicePort
from src.domain.value_objects.invoice_status import InvoiceStatus
from src.shared.config import get_settings
from src.shared.logger import LoggerFactory

logger = LoggerFactory.get_logger(__name__)


class EmailService(EmailServicePort):
    def __init__(self):
        self._settings = get_settings()

    def _build_email_content(self, client_name: str, invoice: Invoice, new_status: InvoiceStatus) -> tuple:
        if new_status == InvoiceStatus.PRIMER_RECORDATORIO:
            return self._build_primer_recordatorio_email(client_name, invoice)
        elif new_status == InvoiceStatus.SEGUNDO_RECORDATORIO:
            return self._build_segundo_recordatorio_email(client_name, invoice)
        else:
            return self._build_desactivado_email(client_name, invoice)

    def _build_primer_recordatorio_email(self, client_name: str, invoice: Invoice) -> tuple:
        subject = f"📋 Primer Recordatorio - Factura {invoice.invoice_number}"
        body = f"""
        <!DOCTYPE html>
        <html lang="es">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
        </head>
        <body style="margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #EFF6FF;">
            <table role="presentation" style="width: 100%; border-collapse: collapse;">
                <tr>
                    <td align="center" style="padding: 40px 0;">
                        <table role="presentation" style="width: 600px; border-collapse: collapse; background-color: #FFFFFF; border-radius: 16px; box-shadow: 0 4px 20px rgba(59, 130, 246, 0.15);">
                            <!-- Header con tema azul -->
                            <tr>
                                <td style="padding: 0; text-align: center;">
                                    <div style="background: linear-gradient(135deg, #3B82F6 0%, #2563EB 100%); border-radius: 16px 16px 0 0; padding: 40px 40px 60px;">
                                        <div style="width: 80px; height: 80px; background-color: rgba(255,255,255,0.2); border-radius: 50%; margin: 0 auto 20px; display: flex; align-items: center; justify-content: center;">
                                            <span style="font-size: 40px;">📋</span>
                                        </div>
                                        <h1 style="margin: 0; color: #FFFFFF; font-size: 26px; font-weight: 700;">Primer Recordatorio</h1>
                                        <p style="margin: 10px 0 0; color: #BFDBFE; font-size: 14px;">Recordatorio de pago pendiente</p>
                                    </div>
                                </td>
                            </tr>
                            
                            <!-- Invoice Card flotante -->
                            <tr>
                                <td style="padding: 0 30px;">
                                    <table role="presentation" style="width: 100%; border-collapse: collapse; background-color: #FFFFFF; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); margin-top: -40px; border: 1px solid #BFDBFE;">
                                        <tr>
                                            <td style="padding: 25px;">
                                                <table role="presentation" style="width: 100%; border-collapse: collapse;">
                                                    <tr>
                                                        <td style="text-align: center; padding-bottom: 15px; border-bottom: 2px dashed #BFDBFE;">
                                                            <p style="margin: 0; color: #1E40AF; font-size: 11px; text-transform: uppercase; letter-spacing: 2px; font-weight: 600;">Número de Factura</p>
                                                            <p style="margin: 8px 0 0; color: #1E293B; font-size: 28px; font-weight: 800;">{invoice.invoice_number}</p>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-top: 20px;">
                                                            <table role="presentation" style="width: 100%; border-collapse: collapse;">
                                                                <tr>
                                                                    <td style="width: 100%; text-align: center; padding: 15px; background-color: #EFF6FF; border-radius: 8px;">
                                                                        <p style="margin: 0; color: #1E40AF; font-size: 11px; text-transform: uppercase; letter-spacing: 1px;">Monto a Pagar</p>
                                                                        <p style="margin: 8px 0 0; color: #2563EB; font-size: 32px; font-weight: 800;">${invoice.amount:,.0f}</p>
                                                                        <p style="margin: 4px 0 0; color: #1E40AF; font-size: 11px;">COP</p>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            
                            <!-- Mensaje -->
                            <tr>
                                <td style="padding: 30px 40px;">
                                    <h2 style="margin: 0 0 15px; color: #1E293B; font-size: 20px; font-weight: 600;">Hola {client_name},</h2>
                                    <p style="margin: 0; color: #64748B; font-size: 15px; line-height: 1.7;">
                                        Le recordamos amablemente que tiene una factura pendiente de pago. 
                                        Por favor, realice el pago a la brevedad para evitar inconvenientes con su servicio.
                                    </p>
                                </td>
                            </tr>
                            
                            <!-- Info -->
                            <tr>
                                <td style="padding: 0 40px 30px;">
                                    <table role="presentation" style="width: 100%; border-collapse: collapse; background-color: #EFF6FF; border-radius: 8px; border-left: 4px solid #3B82F6;">
                                        <tr>
                                            <td style="padding: 15px 20px;">
                                                <p style="margin: 0; color: #1E40AF; font-size: 14px;">
                                                    <strong>💡 Tip:</strong> Realice su pago a tiempo para mantener su servicio activo sin interrupciones.
                                                </p>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            
                            <!-- Botón -->
                            <tr>
                                <td style="padding: 0 40px 40px; text-align: center;">
                                    <a href="#" style="display: inline-block; padding: 18px 50px; background: linear-gradient(135deg, #3B82F6 0%, #2563EB 100%); color: #FFFFFF; font-size: 16px; font-weight: 700; text-decoration: none; border-radius: 50px; box-shadow: 0 4px 15px rgba(59, 130, 246, 0.4);">
                                        💳 Realizar Pago
                                    </a>
                                </td>
                            </tr>
                            
                            <!-- Footer -->
                            <tr>
                                <td style="padding: 30px 40px; text-align: center; background-color: #EFF6FF; border-radius: 0 0 16px 16px;">
                                    <p style="margin: 0 0 10px; color: #1E40AF; font-size: 13px;">
                                        Si ya realizó el pago, por favor ignore este mensaje.
                                    </p>
                                    <p style="margin: 0; color: #2563EB; font-size: 12px;">
                                        ¿Necesitas ayuda? <a href="mailto:soporte@reminderflow.com" style="color: #3B82F6; text-decoration: underline;">Contáctanos</a>
                                    </p>
                                    <hr style="border: none; border-top: 1px solid #BFDBFE; margin: 20px 0;">
                                    <p style="margin: 0; color: #3B82F6; font-size: 11px;">
                                        © 2026 Reminder Flow · Sistema de Gestión de Cobranzas
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </body>
        </html>
        """
        return subject, body

    def _build_segundo_recordatorio_email(self, client_name: str, invoice: Invoice) -> tuple:
        subject = f"⚠️ Segundo Recordatorio - Factura {invoice.invoice_number}"
        body = f"""
        <!DOCTYPE html>
        <html lang="es">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
        </head>
        <body style="margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #FFFBEB;">
            <table role="presentation" style="width: 100%; border-collapse: collapse;">
                <tr>
                    <td align="center" style="padding: 40px 0;">
                        <table role="presentation" style="width: 600px; border-collapse: collapse; background-color: #FFFFFF; border-radius: 16px; box-shadow: 0 4px 20px rgba(245, 158, 11, 0.15);">
                            <!-- Header con tema amarillo/naranja -->
                            <tr>
                                <td style="padding: 0; text-align: center;">
                                    <div style="background: linear-gradient(135deg, #F59E0B 0%, #D97706 100%); border-radius: 16px 16px 0 0; padding: 40px 40px 60px;">
                                        <div style="width: 80px; height: 80px; background-color: rgba(255,255,255,0.2); border-radius: 50%; margin: 0 auto 20px; display: flex; align-items: center; justify-content: center;">
                                            <span style="font-size: 40px;">⚠️</span>
                                        </div>
                                        <h1 style="margin: 0; color: #FFFFFF; font-size: 26px; font-weight: 700;">Segundo Recordatorio</h1>
                                        <p style="margin: 10px 0 0; color: #FEF3C7; font-size: 14px;">Factura pendiente de pago</p>
                                    </div>
                                </td>
                            </tr>
                            
                            <!-- Invoice Card flotante -->
                            <tr>
                                <td style="padding: 0 30px;">
                                    <table role="presentation" style="width: 100%; border-collapse: collapse; background-color: #FFFFFF; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); margin-top: -40px; border: 1px solid #FED7AA;">
                                        <tr>
                                            <td style="padding: 25px;">
                                                <table role="presentation" style="width: 100%; border-collapse: collapse;">
                                                    <tr>
                                                        <td style="text-align: center; padding-bottom: 15px; border-bottom: 2px dashed #FED7AA;">
                                                            <p style="margin: 0; color: #92400E; font-size: 11px; text-transform: uppercase; letter-spacing: 2px; font-weight: 600;">Número de Factura</p>
                                                            <p style="margin: 8px 0 0; color: #1E293B; font-size: 28px; font-weight: 800;">{invoice.invoice_number}</p>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-top: 20px;">
                                                            <table role="presentation" style="width: 100%; border-collapse: collapse;">
                                                                <tr>
                                                                    <td style="width: 50%; text-align: center; padding: 15px; background-color: #FEF3C7; border-radius: 8px;">
                                                                        <p style="margin: 0; color: #92400E; font-size: 11px; text-transform: uppercase; letter-spacing: 1px;">Monto Adeudado</p>
                                                                        <p style="margin: 8px 0 0; color: #B45309; font-size: 32px; font-weight: 800;">${invoice.amount:,.0f}</p>
                                                                        <p style="margin: 4px 0 0; color: #92400E; font-size: 11px;">COP</p>
                                                                    </td>
                                                                    <td style="width: 10px;"></td>
                                                                    <td style="width: 50%; text-align: center; padding: 15px; background-color: #FEF3C7; border-radius: 8px;">
                                                                        <p style="margin: 0; color: #92400E; font-size: 11px; text-transform: uppercase; letter-spacing: 1px;">Fecha Límite</p>
                                                                        <p style="margin: 8px 0 0; color: #B45309; font-size: 20px; font-weight: 700;">📅 {invoice.due_date.strftime('%d/%m/%Y')}</p>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            
                            <!-- Mensaje -->
                            <tr>
                                <td style="padding: 30px 40px;">
                                    <h2 style="margin: 0 0 15px; color: #1E293B; font-size: 20px; font-weight: 600;">Hola {client_name},</h2>
                                    <p style="margin: 0; color: #64748B; font-size: 15px; line-height: 1.7;">
                                        Este es un <strong style="color: #D97706;">segundo recordatorio</strong> de que su factura aún está pendiente de pago. 
                                        Le solicitamos amablemente realizar el pago lo antes posible para evitar la <strong>suspensión del servicio</strong>.
                                    </p>
                                </td>
                            </tr>
                            
                            <!-- Alerta -->
                            <tr>
                                <td style="padding: 0 40px 30px;">
                                    <table role="presentation" style="width: 100%; border-collapse: collapse; background-color: #FEF3C7; border-radius: 8px; border-left: 4px solid #F59E0B;">
                                        <tr>
                                            <td style="padding: 15px 20px;">
                                                <p style="margin: 0; color: #92400E; font-size: 14px;">
                                                    <strong>⏰ Importante:</strong> Si no recibimos el pago pronto, su cuenta será desactivada automáticamente.
                                                </p>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            
                            <!-- Botón -->
                            <tr>
                                <td style="padding: 0 40px 40px; text-align: center;">
                                    <a href="#" style="display: inline-block; padding: 18px 50px; background: linear-gradient(135deg, #F59E0B 0%, #D97706 100%); color: #FFFFFF; font-size: 16px; font-weight: 700; text-decoration: none; border-radius: 50px; box-shadow: 0 4px 15px rgba(245, 158, 11, 0.4);">
                                        💳 Realizar Pago Ahora
                                    </a>
                                </td>
                            </tr>
                            
                            <!-- Footer -->
                            <tr>
                                <td style="padding: 30px 40px; text-align: center; background-color: #FFFBEB; border-radius: 0 0 16px 16px;">
                                    <p style="margin: 0 0 10px; color: #92400E; font-size: 13px;">
                                        Si ya realizó el pago, por favor ignore este mensaje.
                                    </p>
                                    <p style="margin: 0; color: #B45309; font-size: 12px;">
                                        ¿Necesitas ayuda? <a href="mailto:soporte@reminderflow.com" style="color: #D97706; text-decoration: underline;">Contáctanos</a>
                                    </p>
                                    <hr style="border: none; border-top: 1px solid #FED7AA; margin: 20px 0;">
                                    <p style="margin: 0; color: #D97706; font-size: 11px;">
                                        © 2026 Reminder Flow · Sistema de Gestión de Cobranzas
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </body>
        </html>
        """
        return subject, body

    def _build_desactivado_email(self, client_name: str, invoice: Invoice) -> tuple:
        subject = f"🔴 Cuenta Desactivada - Factura {invoice.invoice_number}"
        body = f"""
        <!DOCTYPE html>
        <html lang="es">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
        </head>
        <body style="margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #FEF2F2;">
            <table role="presentation" style="width: 100%; border-collapse: collapse;">
                <tr>
                    <td align="center" style="padding: 40px 0;">
                        <table role="presentation" style="width: 600px; border-collapse: collapse; background-color: #FFFFFF; border-radius: 16px; box-shadow: 0 4px 20px rgba(239, 68, 68, 0.15);">
                            <!-- Header con tema rojo -->
                            <tr>
                                <td style="padding: 0; text-align: center;">
                                    <div style="background: linear-gradient(135deg, #EF4444 0%, #DC2626 100%); border-radius: 16px 16px 0 0; padding: 40px 40px 60px;">
                                        <div style="width: 80px; height: 80px; background-color: rgba(255,255,255,0.2); border-radius: 50%; margin: 0 auto 20px; display: flex; align-items: center; justify-content: center;">
                                            <span style="font-size: 40px;">🚫</span>
                                        </div>
                                        <h1 style="margin: 0; color: #FFFFFF; font-size: 26px; font-weight: 700;">Cuenta Desactivada</h1>
                                        <p style="margin: 10px 0 0; color: #FECACA; font-size: 14px;">Servicio suspendido por falta de pago</p>
                                    </div>
                                </td>
                            </tr>
                            
                            <!-- Invoice Card flotante -->
                            <tr>
                                <td style="padding: 0 30px;">
                                    <table role="presentation" style="width: 100%; border-collapse: collapse; background-color: #FFFFFF; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); margin-top: -40px; border: 1px solid #FECACA;">
                                        <tr>
                                            <td style="padding: 25px;">
                                                <table role="presentation" style="width: 100%; border-collapse: collapse;">
                                                    <tr>
                                                        <td style="text-align: center; padding-bottom: 15px; border-bottom: 2px dashed #FECACA;">
                                                            <p style="margin: 0; color: #991B1B; font-size: 11px; text-transform: uppercase; letter-spacing: 2px; font-weight: 600;">Factura Vencida</p>
                                                            <p style="margin: 8px 0 0; color: #1E293B; font-size: 28px; font-weight: 800;">{invoice.invoice_number}</p>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-top: 20px;">
                                                            <table role="presentation" style="width: 100%; border-collapse: collapse;">
                                                                <tr>
                                                                    <td style="width: 50%; text-align: center; padding: 15px; background-color: #FEE2E2; border-radius: 8px;">
                                                                        <p style="margin: 0; color: #991B1B; font-size: 11px; text-transform: uppercase; letter-spacing: 1px;">Deuda Total</p>
                                                                        <p style="margin: 8px 0 0; color: #DC2626; font-size: 32px; font-weight: 800;">${invoice.amount:,.0f}</p>
                                                                        <p style="margin: 4px 0 0; color: #991B1B; font-size: 11px;">COP</p>
                                                                    </td>
                                                                    <td style="width: 10px;"></td>
                                                                    <td style="width: 50%; text-align: center; padding: 15px; background-color: #FEE2E2; border-radius: 8px;">
                                                                        <p style="margin: 0; color: #991B1B; font-size: 11px; text-transform: uppercase; letter-spacing: 1px;">Estado</p>
                                                                        <p style="margin: 8px 0 0; color: #DC2626; font-size: 18px; font-weight: 700;">⛔ SUSPENDIDO</p>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            
                            <!-- Mensaje -->
                            <tr>
                                <td style="padding: 30px 40px;">
                                    <h2 style="margin: 0 0 15px; color: #1E293B; font-size: 20px; font-weight: 600;">Estimado/a {client_name},</h2>
                                    <p style="margin: 0; color: #64748B; font-size: 15px; line-height: 1.7;">
                                        Lamentamos informarle que debido al <strong style="color: #DC2626;">incumplimiento en el pago</strong> de su factura, 
                                        su cuenta ha sido <strong style="color: #DC2626;">desactivada</strong>. Todos los servicios asociados han sido suspendidos.
                                    </p>
                                </td>
                            </tr>
                            
                            <!-- Pasos para reactivar -->
                            <tr>
                                <td style="padding: 0 40px 30px;">
                                    <table role="presentation" style="width: 100%; border-collapse: collapse; background-color: #F8FAFC; border-radius: 12px;">
                                        <tr>
                                            <td style="padding: 25px;">
                                                <p style="margin: 0 0 15px; color: #1E293B; font-size: 14px; font-weight: 700;">📋 Para reactivar su cuenta:</p>
                                                <table role="presentation" style="width: 100%; border-collapse: collapse;">
                                                    <tr>
                                                        <td style="padding: 8px 0; vertical-align: top; width: 30px;">
                                                            <span style="display: inline-block; width: 24px; height: 24px; background-color: #EF4444; color: white; border-radius: 50%; text-align: center; line-height: 24px; font-size: 12px; font-weight: 700;">1</span>
                                                        </td>
                                                        <td style="padding: 8px 0; color: #64748B; font-size: 14px;">Realice el pago total de la deuda pendiente</td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding: 8px 0; vertical-align: top; width: 30px;">
                                                            <span style="display: inline-block; width: 24px; height: 24px; background-color: #EF4444; color: white; border-radius: 50%; text-align: center; line-height: 24px; font-size: 12px; font-weight: 700;">2</span>
                                                        </td>
                                                        <td style="padding: 8px 0; color: #64748B; font-size: 14px;">Envíe el comprobante de pago a nuestro equipo</td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding: 8px 0; vertical-align: top; width: 30px;">
                                                            <span style="display: inline-block; width: 24px; height: 24px; background-color: #EF4444; color: white; border-radius: 50%; text-align: center; line-height: 24px; font-size: 12px; font-weight: 700;">3</span>
                                                        </td>
                                                        <td style="padding: 8px 0; color: #64748B; font-size: 14px;">Su servicio será reactivado en 24-48 horas</td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            
                            <!-- Botones -->
                            <tr>
                                <td style="padding: 0 40px 40px; text-align: center;">
                                    <a href="#" style="display: inline-block; padding: 18px 40px; background: linear-gradient(135deg, #EF4444 0%, #DC2626 100%); color: #FFFFFF; font-size: 16px; font-weight: 700; text-decoration: none; border-radius: 50px; box-shadow: 0 4px 15px rgba(239, 68, 68, 0.4); margin-right: 10px;">
                                        💳 Pagar Ahora
                                    </a>
                                    <a href="#" style="display: inline-block; padding: 18px 40px; background: #FFFFFF; color: #DC2626; font-size: 16px; font-weight: 700; text-decoration: none; border-radius: 50px; border: 2px solid #EF4444;">
                                        📞 Contactar Soporte
                                    </a>
                                </td>
                            </tr>
                            
                            <!-- Footer -->
                            <tr>
                                <td style="padding: 30px 40px; text-align: center; background-color: #FEF2F2; border-radius: 0 0 16px 16px;">
                                    <p style="margin: 0 0 10px; color: #991B1B; font-size: 13px;">
                                        Este es un mensaje automático. Por favor no responda a este correo.
                                    </p>
                                    <p style="margin: 0; color: #B91C1C; font-size: 12px;">
                                        Línea de atención: <strong>+57 300 123 4567</strong> · <a href="mailto:cobranzas@reminderflow.com" style="color: #DC2626; text-decoration: underline;">cobranzas@reminderflow.com</a>
                                    </p>
                                    <hr style="border: none; border-top: 1px solid #FECACA; margin: 20px 0;">
                                    <p style="margin: 0; color: #DC2626; font-size: 11px;">
                                        © 2026 Reminder Flow · Sistema de Gestión de Cobranzas
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </body>
        </html>
        """
        return subject, body

    async def send_reminder_email(
        self,
        to_email: str,
        client_name: str,
        invoice: Invoice,
        new_status: InvoiceStatus
    ) -> bool:
        logger.info(f"Sending reminder email to {to_email} for invoice {invoice.invoice_number}")
        
        try:
            subject, body = self._build_email_content(client_name, invoice, new_status)
            
            message = MIMEMultipart("alternative")
            message["From"] = self._settings.smtp_from
            message["To"] = to_email
            message["Subject"] = subject
            
            html_part = MIMEText(body, "html")
            message.attach(html_part)
            
            if self._settings.smtp_user and self._settings.smtp_password:
                await aiosmtplib.send(
                    message,
                    hostname=self._settings.smtp_host,
                    port=self._settings.smtp_port,
                    username=self._settings.smtp_user,
                    password=self._settings.smtp_password,
                    start_tls=True
                )
                logger.info(f"Email sent successfully to {to_email}")
            else:
                logger.warning(f"SMTP not configured. Email would be sent to {to_email} with subject: {subject}")
            
            return True
        except Exception as e:
            logger.error(f"Failed to send email to {to_email}: {str(e)}")
            return False

using BL;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;   
//AL SER UNA INTERFAZ SE DEBERA INVOCAR A TODOS SUS METODOS
public class ReporteEjecutivoDocument : IDocument
{
    private readonly ReporteEjecutivoViewModel _vm;

    public ReporteEjecutivoDocument(ReporteEjecutivoViewModel vm)
    {
        _vm = vm;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(30);
            page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(10));

            page.Header().Column(col =>
            {
                col.Item().Text("REPORTE EJECUTIVO").FontSize(20).Bold();
                col.Item().Text("Inventario Físico — Control de Stock").FontSize(11).FontColor(Colors.Grey.Darken1);
            });

            page.Content().PaddingVertical(10).Column(col =>
            {
                col.Spacing(15);
                col.Item().Element(ComponentInfoGeneral);
                col.Item().Element(ComponentKpis);
                col.Item().Element(ComponentResumenDiferencias);
                col.Item().Element(ComponentTopProductos);
                col.Item().Element(ComponentTopUbicaciones);
                col.Item().Element(ComponentTopUsuarios);
                col.Item().Element(ComponentConclusiones);
            });

            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("Reporte Ejecutivo de Inventario | Página ");
                x.CurrentPageNumber();
                x.Span(" de ");
                x.TotalPages();
            });
        });
    }

    private void ComponentInfoGeneral(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text("Información General").FontSize(13).Bold();
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn();
                });

                table.Cell().Element(CellHeaderStyle).Text("Inventario");
                table.Cell().Element(CellHeaderStyle).Text("Almacén");
                table.Cell().Element(CellHeaderStyle).Text("Fecha Inicio");
                table.Cell().Element(CellHeaderStyle).Text("Fecha Fin");
                table.Cell().Element(CellHeaderStyle).Text("Duración");

                table.Cell().Element(CellStyle).Text(_vm.CodInventario);
                table.Cell().Element(CellStyle).Text(_vm.DscAlmacen ?? _vm.CodAlmacen);
                table.Cell().Element(CellStyle).Text(_vm.FechaInicio.ToString("dd/MM/yyyy"));
                table.Cell().Element(CellStyle).Text(_vm.FechaCierreFinal?.ToString("dd/MM/yyyy") ?? "-");
                table.Cell().Element(CellStyle).Text($"{_vm.DuracionDias} días");
            });
        });
    }

    private void ComponentKpis(IContainer container)
    {
        container.Row(row =>
        {
            row.Spacing(10);
            row.RelativeItem().Element(c => KpiBox(c, _vm.ProductosInventariados.ToString("N0"), "Productos Inventariados"));
            row.RelativeItem().Element(c => KpiBox(c, _vm.UbicacionesConDiferencias.ToString("N0"), "Ubicaciones con Diferencias"));
            row.RelativeItem().Element(c => KpiBox(c, _vm.UsuariosParticipantes.ToString("N0"), "Usuarios Participantes"));
            row.RelativeItem().Element(c => KpiBox(c, _vm.ProductosConDiferencia.ToString("N0"), "Productos con Diferencia"));
            row.RelativeItem().Element(c => KpiBox(c, $"{_vm.ExactitudPct:0.00}%", "Exactitud del Inventario"));
        });
    }

    private void KpiBox(IContainer container, string valor, string etiqueta)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Column(col =>
        {
            col.Item().AlignCenter().Text(valor).FontSize(18).Bold();
            col.Item().AlignCenter().Text(etiqueta).FontSize(8).FontColor(Colors.Grey.Darken1);
        });
    }

    private void ComponentResumenDiferencias(IContainer container)
    {
        container.Row(row =>
        {
            row.Spacing(10);
            row.RelativeItem().Element(c => KpiBox(c, $"+{_vm.SumaSobrantes:N0}", "Sobrantes"));
            row.RelativeItem().Element(c => KpiBox(c, $"{_vm.SumaFaltantes:N0}", "Faltantes"));
            row.RelativeItem().Element(c => KpiBox(c, $"{_vm.DiferenciaNeta:N0}", "Diferencia Neta"));
        });
    }

    private void ComponentTopProductos(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text("Top 10 Productos con Mayor Diferencia").FontSize(12).Bold();
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(4); c.RelativeColumn(1); });
                table.Cell().Element(CellHeaderStyle).Text("Producto");
                table.Cell().Element(CellHeaderStyle).Text("Diferencia");

                foreach (var p in _vm.Top10Productos)
                {
                    table.Cell().Element(CellStyle).Text(p.DscProducto);
                    table.Cell().Element(CellStyle).Text(p.Diferencia.ToString("+#,##0;-#,##0;0"));
                }
            });
        });
    }

    private void ComponentTopUbicaciones(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text("Top 10 Ubicaciones con Más Diferencias").FontSize(12).Bold();
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(2); c.RelativeColumn(1); c.RelativeColumn(1); });
                table.Cell().Element(CellHeaderStyle).Text("Ubicación");
                table.Cell().Element(CellHeaderStyle).Text("Diferencia Neta");
                table.Cell().Element(CellHeaderStyle).Text("Diferencia Abs.");

                foreach (var u in _vm.Top10Ubicaciones)
                {
                    table.Cell().Element(CellStyle).Text(u.Codigo);
                    table.Cell().Element(CellStyle).Text(u.Diferencia.ToString("+#,##0;-#,##0;0"));
                    table.Cell().Element(CellStyle).Text(Math.Abs(u.Diferencia).ToString("N0"));
                }
            });
        });
    }

    private void ComponentTopUsuarios(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text("Top Usuarios por Cantidad de Registros").FontSize(12).Bold();
            col.Item().Text("Esta métrica muestra la distribución del trabajo, no busca evaluar al usuario.")
                .FontSize(8).Italic().FontColor(Colors.Grey.Darken1);
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(3); c.RelativeColumn(1); });
                table.Cell().Element(CellHeaderStyle).Text("Usuario");
                table.Cell().Element(CellHeaderStyle).Text("Lecturas");

                foreach (var u in _vm.TopUsuarios)
                {
                    table.Cell().Element(CellStyle).Text(!string.IsNullOrWhiteSpace(u.NombreCompleto) ? u.NombreCompleto : u.CodUsuario);
                    table.Cell().Element(CellStyle).Text(u.CantLecturas.ToString());
                }
            });
        });
    }

    private void ComponentConclusiones(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text("Conclusiones").FontSize(13).Bold();
            col.Item().Text("Resumen General").FontSize(11).Bold();
            col.Item().Text(_vm.ResumenEjecutivo).FontSize(9);
        });
    }

    private static IContainer CellHeaderStyle(IContainer container) =>
        container.Background(Colors.Grey.Lighten3).Padding(4).DefaultTextStyle(x => x.Bold().FontSize(9));

    private static IContainer CellStyle(IContainer container) =>
        container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).DefaultTextStyle(x => x.FontSize(9));

}
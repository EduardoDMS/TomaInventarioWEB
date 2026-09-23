using BL;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;

// AL SER UNA INTERFAZ SE DEBERA INVOCAR A TODOS SUS METODOS
public class ReporteEjecutivoDocument : IDocument
{
    private readonly ReporteEjecutivoViewModel _vm;

    private static readonly string AzulOscuro = "#0D3B66";
    private static readonly string Celeste = "#2E86AB";
    private static readonly string CelesteClaro = "#EAF3FA";
    private static readonly string GrisTexto = "#4A4A4A";

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

            //page.Header().Element(ComponentHeader);

            page.Content().PaddingVertical(10).Column(col =>
            {
                col.Spacing(15);

                col.Item().Element(ComponentHeader);

                col.Item().Element(ComponentInfoGeneral);
                col.Item().Element(ComponentKpisFila1);
                col.Item().Element(ComponentKpisFila2);
                col.Item().Element(ComponentTopProductos);
                col.Item().Element(ComponentTopUbicaciones);
                col.Item().Element(ComponentTopFueraUbicacion);
                col.Item().Element(ComponentUsuarios);
                col.Item().Element(ComponentConclusiones);
            });

            page.Footer().Column(col =>
            {
                col.Item().PaddingBottom(4).LineHorizontal(0.5f).LineColor(Color.FromHex(Celeste));
                col.Item().AlignCenter().Text(x =>
                {
                    x.DefaultTextStyle(y => y.FontSize(8).FontColor(Color.FromHex(GrisTexto)));
                    x.Span("Reporte Ejecutivo de Inventario | Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        });
    }

    private void ComponentHeader(IContainer container)
    {
        container.Background(Color.FromHex(AzulOscuro)).Padding(15).Column(col =>
        {
            col.Item().Text("REPORTE EJECUTIVO").FontSize(20).Bold().FontColor(Colors.White);
            col.Item().Text("Inventario Físico").FontSize(11).FontColor(Color.FromHex("#BFD7EA"));
        });
    }

    private void SectionTitle(IContainer container, string texto)
    {
        container.Row(row =>
        {
            row.AutoItem().Width(3).Background(Color.FromHex(Celeste));
            row.RelativeItem().PaddingLeft(6).Text(texto).FontSize(13).Bold().FontColor(Color.FromHex(AzulOscuro));
        });
    }

    private void ComponentInfoGeneral(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionTitle(c, "Información General"));
            col.Item().PaddingTop(6).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.RelativeColumn();
                });

                table.Cell().Element(CellHeaderStyle).AlignCenter().Text("Inventario");
                table.Cell().Element(CellHeaderStyle).AlignCenter().Text("Almacén");
                table.Cell().Element(CellHeaderStyle).AlignCenter().Text("Fecha Inicio");
                table.Cell().Element(CellHeaderStyle).AlignCenter().Text("Fecha Fin");
                table.Cell().Element(CellHeaderStyle).AlignCenter().Text("Conteos Realizados");
                table.Cell().Element(CellHeaderStyle).AlignCenter().Text("Duración");
                table.Cell().Element(CellHeaderStyle).AlignCenter().Text("Operadores Participantes");

                table.Cell().Element(CellStyle).AlignCenter().Text(_vm.CodInventario);
                table.Cell().Element(CellStyle).AlignCenter().Text(_vm.DscAlmacen ?? _vm.CodAlmacen);
                table.Cell().Element(CellStyle).AlignCenter().Text(_vm.FechaInicio.ToString("dd/MM/yyyy"));
                table.Cell().Element(CellStyle).AlignCenter().Text(_vm.FechaCierreFinal?.ToString("dd/MM/yyyy") ?? "-");
                table.Cell().Element(CellStyle).AlignCenter().Text(_vm.ConteosRealizados.ToString());
                table.Cell().Element(CellStyle).AlignCenter().Text($"{_vm.DuracionHoras}");
                table.Cell().Element(CellStyle).AlignCenter().Text(_vm.UsuariosParticipantes.ToString("N0"));
            });
        });
    }

    private void ComponentKpisFila1(IContainer container)
    {
        container.Row(row =>
        {
            row.Spacing(10);
            row.RelativeItem().Element(c => KpiBox(c, $"{_vm.ExactitudPct:0.00}%", "Exactitud Por productos", true));
            row.RelativeItem().Element(c => KpiBox(c, $"{_vm.ExactitudPorUnidades:0.00}%", "Exactitud por Stock", true));
            row.RelativeItem().Element(c => KpiBox(c, _vm.ProductosInventariados.ToString("N0"), "Productos Inventariados", false));
            row.RelativeItem().Element(c => KpiBox(c, _vm.ProductosFaltantes.ToString("N0"), "Productos Faltantes", false));
            row.RelativeItem().Element(c => KpiBox(c, _vm.ProductosSobrantes.ToString("N0"), "Productos Sobrantes", false));
            row.RelativeItem().Element(c => KpiBox(c, _vm.ProductosFueraUbicacion.ToString("N0"), "Productos Fuera de Ubicación", false));
        });
    }

    private void ComponentKpisFila2(IContainer container)
    {
        container.Row(row =>
        {
            row.Spacing(10);
            row.RelativeItem().Element(c => KpiBox(c, $"-{Math.Abs(_vm.SumaFaltantes):N0}", "Stock Faltante", false));
            row.RelativeItem().Element(c => KpiBox(c, $"+{_vm.SumaSobrantes:N0}", "Stock Sobrante", false));
        });
    }
    private void KpiBox(IContainer container, string valor, string etiqueta, bool destacado)
    {
        container
            .Border(1)
            .BorderColor(Color.FromHex("#D9E4EC"))
            .BorderTop(destacado ? 3 : 1)
            .BorderColor(destacado ? Color.FromHex(Celeste) : Color.FromHex("#D9E4EC"))
            .Padding(8)
            .Column(col =>
            {
                col.Item().AlignCenter().Text(valor).FontSize(18).Bold().FontColor(Color.FromHex(AzulOscuro));
                col.Item().AlignCenter().Text(etiqueta).FontSize(8).FontColor(Color.FromHex(GrisTexto));
            });
    }

    private void ComponentTopProductos(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionTitle(c, "Top 10 Productos con Mayor Diferencia"));

            if (!_vm.Top10Productos.Any())
            {
                col.Item().PaddingTop(6).Text("No se registraron diferencias en productos.").FontSize(9).Italic().FontColor(Color.FromHex(GrisTexto));
                return;
            }

            col.Item().PaddingTop(6).Table(table =>
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
            col.Item().Element(c => SectionTitle(c, "Top 10 Ubicaciones con Más Diferencias"));

            if (!_vm.Top10Ubicaciones.Any())
            {
                col.Item().PaddingTop(6).Text("No se registraron diferencias en ubicaciones.").FontSize(9).Italic().FontColor(Color.FromHex(GrisTexto));
                return;
            }

            col.Item().PaddingTop(6).Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(2); c.RelativeColumn(1); c.RelativeColumn(1); c.RelativeColumn(1); });
                table.Cell().Element(CellHeaderStyle).Text("Ubicación");
                table.Cell().Element(CellHeaderStyle).Text("Stock Inicial");
                table.Cell().Element(CellHeaderStyle).Text("Stock Contado");
                table.Cell().Element(CellHeaderStyle).Text("Diferencia");

                foreach (var u in _vm.Top10Ubicaciones)
                {
                    table.Cell().Element(CellStyle).Text(u.Codigo);
                    table.Cell().Element(CellStyle).Text(u.StockInicial.ToString("N0"));
                    table.Cell().Element(CellStyle).Text(u.StockFinal.ToString("N0"));
                    table.Cell().Element(CellStyle).Text(u.Diferencia.ToString("+#,##0;-#,##0;0"));
                }
            });
        });
    }

    private void ComponentTopFueraUbicacion(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionTitle(c, "Top 10 Productos Fuera de su Ubicación"));

            if (!_vm.Top10ProductosFueraUbicacion.Any())
            {
                col.Item().PaddingTop(6).Text("No se registraron productos fuera de ubicación.").FontSize(9).Italic().FontColor(Color.FromHex(GrisTexto));
                return;
            }

            col.Item().PaddingTop(6).Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(3); c.RelativeColumn(1); c.RelativeColumn(1); });
                table.Cell().Element(CellHeaderStyle).Text("Producto");
                table.Cell().Element(CellHeaderStyle).Text("Ubicación Inicial");
                table.Cell().Element(CellHeaderStyle).Text("Ubicación Contada");

                foreach (var p in _vm.Top10ProductosFueraUbicacion)
                {
                    table.Cell().Element(CellStyle).Text(p.DscProducto);
                    table.Cell().Element(CellStyle).Text(p.UbicacionInicial ?? "-");
                    table.Cell().Element(CellStyle).Text(p.UbicacionContada ?? "-");
                }
            });
        });
    }

    private void ComponentUsuarios(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionTitle(c, "Usuarios por Cantidad de Productos"));
            col.Item().PaddingTop(2).Text("Esta métrica muestra la distribución del trabajo, no busca evaluar al usuario.")
                .FontSize(8).Italic().FontColor(Color.FromHex(GrisTexto));

            if (!_vm.Usuarios.Any())
            {
                col.Item().PaddingTop(6).Text("No se registró participación de usuarios.").FontSize(9).Italic().FontColor(Color.FromHex(GrisTexto));
                return;
            }

            col.Item().PaddingTop(6).Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(3); c.RelativeColumn(1); });
                table.Cell().Element(CellHeaderStyle).Text("Usuario");
                table.Cell().Element(CellHeaderStyle).Text("Productos Lecturados");

                foreach (var u in _vm.Usuarios)
                {
                    table.Cell().Element(CellStyle).Text(!string.IsNullOrWhiteSpace(u.NombreCompleto) ? u.NombreCompleto : u.CodUsuario);
                    table.Cell().Element(CellStyle).Text(u.ProductosLecturados.ToString());
                }
            });
        });
    }

    private void ComponentConclusiones(IContainer container)
    {
        container
            .Background(Color.FromHex(CelesteClaro))
            .Padding(10)
            .Column(col =>
            {
                col.Item().Element(c => SectionTitle(c, "Conclusiones"));
                col.Item().PaddingTop(6).Text("Resumen General").FontSize(11).Bold().FontColor(Color.FromHex(AzulOscuro));
                col.Item().Text(_vm.ResumenEjecutivo).FontSize(9).FontColor(Color.FromHex(GrisTexto));
            });
    }
    private static IContainer CellHeaderStyle(IContainer container) =>
        container.Background(Color.FromHex(CelesteClaro)).Padding(4)
            .DefaultTextStyle(x => x.Bold().FontSize(9).FontColor(Color.FromHex(AzulOscuro)));

    private static IContainer CellStyle(IContainer container) =>
        container.BorderBottom(1).BorderColor(Color.FromHex("#E3E3E3")).Padding(4)
            .DefaultTextStyle(x => x.FontSize(9).FontColor(Color.FromHex(GrisTexto)));
}
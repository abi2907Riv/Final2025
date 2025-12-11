async function generarGraficoBarra() {

    const res = await authFetch(`Actividades/caloriasquemadas`)

    const Meses = await res.json();

    console.log(Meses);

    const ctx = document.getElementById('grafico');

    let labels = [];
    let data = [];

    Meses.forEach(dia => {
        let fecha = new Date(dia.fecha).toLocaleDateString();
        labels.push(fecha);
        data.push(dia.calorias);
    });

    contenidoGrafico = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Calorias por día',
                data: data,
                fill: false,
                borderColor: 'rgb(75, 192, 192)',
                tension: 0.1
            }],
        },
        options: {
            responsive: true, //Hace que el gráfico se adapte automáticamente al tamaño del contenedor 
            maintainAspectRatio: false, //Al poner false, le decís que no mantenga esa proporción y que llene el espacio disponible del contenedor

        }
    });
}

generarGraficoBarra();
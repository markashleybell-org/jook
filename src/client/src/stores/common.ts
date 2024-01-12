// TODO: Move into config
const baseUrl = 'https://localhost:5001'

export async function get(url: string) {
    const response = await fetch(baseUrl + url, {
        method: 'GET',
        headers: {
            "Accept": "application/json",
            // ApiKey: apiKey,
        }
    });
    const json = await response.json();
    return response.ok ? json : Promise.reject(json);
}

export async function post(url: string, payload: any) {
    const response = await fetch(baseUrl + url, {
        method: 'POST',
        body: JSON.stringify(payload),
        headers: {
            "Content-Type": "application/json",
            // ApiKey: apiKey,
        },
    });
    const json = await response.json();
    return response.ok ? json : Promise.reject(json);
}

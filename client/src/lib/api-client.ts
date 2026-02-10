export class ApiError extends Error {
  status: number;
  code: string;
  description: string;

  constructor(status: number, code: string, description: string) {
    super(description);
    this.name = 'ApiError';
    this.status = status;
    this.code = code;
    this.description = description;
  }
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    let code = 'UNKNOWN_ERROR';
    let description = 'Ha ocurrido un error inesperado';

    try {
      const body = await response.json();
      code = body.code ?? body.title ?? code;
      description = body.description ?? body.detail ?? description;
    } catch {
      /* empty */
    }

    throw new ApiError(response.status, code, description);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json();
}

export const api = {
  async get<T>(url: string): Promise<T> {
    const response = await fetch(url, {
      headers: { 'Content-Type': 'application/json' },
    });
    return handleResponse<T>(response);
  },

  async post<T>(url: string, body?: unknown): Promise<T> {
    const response = await fetch(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: body !== undefined ? JSON.stringify(body) : undefined,
    });
    return handleResponse<T>(response);
  },

  async put<T>(url: string, body?: unknown): Promise<T> {
    const response = await fetch(url, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: body !== undefined ? JSON.stringify(body) : undefined,
    });
    return handleResponse<T>(response);
  },

  async delete<T>(url: string): Promise<T> {
    const response = await fetch(url, {
      method: 'DELETE',
      headers: { 'Content-Type': 'application/json' },
    });
    return handleResponse<T>(response);
  },
};

export const isLightSquare = (position: number[], index: number): boolean => {
    const row = position[1]; // Baris (1-8)
    const isEven = (x: number) => !(x % 2); // Fungsi untuk mengecek apakah angka genap

    // Logika untuk menentukan kotak terang atau gelap
    if (isEven(row) && !isEven(index + 1)) {
        return true;
    }
    if (isEven(index + 1) && !isEven(row)) {
        return true;
    }
    return false;
};
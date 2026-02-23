export interface ArtPiece {
  id: number;
  name: string;
  description: string;
  price: number;
  isAvailable: boolean;
  imageUrl: string;
}

export interface CommissionType {
  id: number;
  medium: string;
  price: number;
}

export interface Commission {
  id: number;
  userId: string;
  name: string;
  description: string;
  typeId: number;
  price: number;
  isCompleted: boolean;
  type?: CommissionType;
}

export interface Invoice {
  id: number;
  userId: string;
  commissionId: number;
  totalPrice: number;
  commission?: Commission;
}

import { BaseModel } from './base.model';

export class Comment extends BaseModel {
  name!: string;
  body!: string;
  creationDate!: string;
  likesCount!: number
  childComments?: Comment[];
}

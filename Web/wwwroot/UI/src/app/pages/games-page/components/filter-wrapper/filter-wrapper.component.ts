import { HttpParams, HttpUrlEncodingCodec } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, FormArray } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { forkJoin, BehaviorSubject, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/components/base.component';
import { Genre } from 'src/app/models/genre.model';
import { Platform } from 'src/app/models/platform.model';
import { Publisher } from 'src/app/models/publisher.model';
import { GameService } from 'src/app/services/game.service';
import { GenreService } from 'src/app/services/genre.service';
import { PlatformService } from 'src/app/services/platform.service';
import { PublisherService } from 'src/app/services/publisher.service';
import { FilterTrigger } from './filter-trigger';

@UntilDestroy()
@Component({
  selector: 'gamestore-filter-wrapper',
  templateUrl: './filter-wrapper.component.html',
  styleUrls: ['./filter-wrapper.component.scss'],
})
export class FilterWrapperComponent extends BaseComponent implements OnInit {
  private readonly unsubscribe = new Subject<void>();

  @Input()
  totalPages: number = 1;

  @Input()
  currentPage: number = 1;

  @Input()
  pagesSubject!: BehaviorSubject<{ totalPages: number; page: number }>;

  @Input()
  gamesLoadSubject!: Subject<void>;

  @Input()
  pageTitle!: string;

  @Output()
  apply = new EventEmitter<HttpParams>();

  form?: FormGroup;

  genreItems: string[] = [];
  platformItems: string[] = [];
  publisherItems: string[] = [];
  datesPublishingItems: { value: string; title: string }[] = [];

  genres: Genre[] = [];
  publishers: Publisher[] = [];
  platforms: Platform[] = [];
  datesPublishing: string[] = [];

  constructor(
    private genreService: GenreService,
    private platformService: PlatformService,
    private publisherService: PublisherService,
    private gameService: GameService,
    private route: ActivatedRoute,
    private builder: FormBuilder
  ) {
    super();
  }

  ngOnInit(): void {
    forkJoin({
      genres: this.genreService.getGenres(),
      platforms: this.platformService.getPlatforms(),
      publishers: this.publisherService.getPublishers(),
      datesPublishing: this.gameService.getPublishedDateOptions(),
    }).subscribe((x) => {
      this.platforms = x.platforms;
      this.publishers = x.publishers;
      this.genres = x.genres;
      this.datesPublishing = x.datesPublishing;

      this.createForm(
        (this.route.queryParams as BehaviorSubject<Params>).getValue()
      );
      this.applyFilters(FilterTrigger.ApplyFilters);
    });

    this.pagesSubject.pipe(untilDestroyed(this)).subscribe((x) => {
      this.totalPages = x.totalPages;
      this.getFormControl('page')?.setValue(x.page);
    });

    this.gamesLoadSubject.pipe(untilDestroyed(this)).subscribe((_) => {
      this.form?.enable();
      this.subscribeControlChanges();
    });
  }

  getFormControl(name: string): FormControl {
    return this.form?.get(name) as FormControl;
  }

  getFormControlArray(name: string): FormControl[] {
    return (this.form?.get(name) as FormArray).controls.map(
      (x) => x as FormControl
    );
  }

  applyFilters(trigger: FilterTrigger = FilterTrigger.ApplyFilters): void {
    if (this.form?.disabled) return;
    this.unsubscribe.next();
    this.form?.disable();

    const selectedGenres = this.genres
        .filter((x, i) => !!this.form!.value.genres[i])
        .map((x) => x.name!);
    const selectedPlatforms = this.platforms
        .filter((x, i) => !!this.form!.value.platforms[i])
        .map((x) => x.type!);
    const selectedPublishers = this.publishers
        .filter((x, i) => !!this.form!.value.publishers[i])
        .map((x) => x.id!);

    const minPrice = this.form!.value.minPrice;
    const maxPrice = this.form!.value.maxPrice;
    const namePart = this.form!.value.name;
    const publishDateOption = this.form!.value.datePublishing;
    const sortingOption = this.form!.value.sort;
    const page = this.form!.value.page;
    const pageSize = this.form!.value.pageCount;

    let params = new HttpParams({ encoder: new HttpUrlEncodingCodec() });

    if (selectedGenres.length > 0) {
      selectedGenres.forEach(g => params = params.append('Genres', g));
    }

    if (selectedPlatforms.length > 0) {
      selectedPlatforms.forEach(p => params = params.append('Platforms', p));
    }

    if (selectedPublishers.length > 0) {
      selectedPublishers.forEach(pu => params = params.append('Publishers', pu));
    }

    if (minPrice !== '' && minPrice != null) params = params.set('MinPrice', minPrice);
    if (maxPrice !== '' && maxPrice != null) params = params.set('MaxPrice', maxPrice);
    if (namePart) params = params.set('NamePart', namePart);
    if (publishDateOption) params = params.set('PublishDateOption', publishDateOption);
    if (sortingOption) params = params.set('SortingOption', sortingOption);

    params = params.set('Page', page.toString());
    if (pageSize !== '' && pageSize != null) {
      params = params.set('PageSize', pageSize);
    }

    params = params.set('trigger', trigger.toString());

    this.apply.emit(params);
  }

  private createForm(params: Params): void {
    this.form = this.builder.group({
      genres: this.builder.array(
        this.genres.map(
          (x) => !!(params['genres'] as string[])?.includes(x.id!)
        )
      ),
      platforms: this.builder.array(
        this.platforms.map(
          (x) => !!(params['platforms'] as string[])?.includes(x.id!)
        )
      ),
      publishers: this.builder.array(
        this.publishers.map(
          (x) => !!(params['publishers'] as string[])?.includes(x.id!)
        )
      ),
      maxPrice: [params['maxPrice'] ?? ''],
      minPrice: [params['minPrice'] ?? ''],
      name: [params['name'] ?? ''],
      datePublishing: [params['datePublishing'] ?? ''],
      sort: [params['sort'] ?? ''],
      page: [Number.parseInt(params['page'] ?? this.currentPage.toString())],
      pageCount: [params['pageCount'] ?? ''],
    });

    this.genreItems = this.genres.map((x) => x.name);
    this.platformItems = this.platforms.map((x) => x.type);
    this.publisherItems = this.publishers.map((x) => x.companyName);
    this.datesPublishingItems = this.datesPublishing.map((x) => {
      return { value: x, title: x };
    });

    this.subscribeControlChanges();
  }

  private subscribeControlChanges(): void {
    this.getFormControl('sort')
      .valueChanges.pipe(takeUntil(this.unsubscribe))
      .subscribe((_) => this.applyFilters(FilterTrigger.SortingChange));
    this.getFormControl('page')
      .valueChanges.pipe(takeUntil(this.unsubscribe))
      .subscribe((_) => this.applyFilters(FilterTrigger.PageChange));
    this.getFormControl('pageCount')
      .valueChanges.pipe(takeUntil(this.unsubscribe))
      .subscribe((_) => this.applyFilters(FilterTrigger.PageCountChange));
  }
}
